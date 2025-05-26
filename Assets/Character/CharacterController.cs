using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float WalkSpeed = 0.5f;
    public float SprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 1f;

    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;
    private bool isOnLadder = false;

    private const float gravity = -20f;

    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public float CurrentYaw => yaw;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        IsGrounded = characterController.isGrounded;

        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        IsMoving = inputDirection.magnitude > 0.1f;
        CurrentInput = new Vector2(horizontal, vertical);

        Vector3 moveDirection = Vector3.zero;

        if (IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
        }

        if (isOnLadder)
        {
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 ladderMovement = new Vector3(0f, verticalInput * currentSpeed, 0f);
            characterController.Move(ladderMovement * Time.deltaTime);

            animator?.SetBool("isOnLadder", true);
            return; // Salimos para no aplicar gravedad
        }

        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetBool("isJumping", true);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = moveDirection * currentSpeed * Time.deltaTime;
        finalMovement.y += velocity.y * Time.deltaTime;

        characterController.Move(finalMovement);

        if (IsGrounded && velocity.y < 0f)
        {
            animator?.SetBool("isJumping", false);
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        yaw += mouseX * mouseSensitivity;

        if (IsMoving)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            velocity = Vector3.zero;

            // Centrar jugador en la escalera (X,Z)
            Vector3 centerXZ = new Vector3(other.bounds.center.x, transform.position.y, other.bounds.center.z);
            transform.position = centerXZ;

            animator?.SetBool("isOnLadder", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            velocity = Vector3.zero;

            animator?.SetBool("isOnLadder", false);
        }
    }

    void UpdateAnimator()
    {
        float inputX = Input.GetAxis("Horizontal");
    float inputZ = Input.GetAxis("Vertical");
    float inputMagnitude = new Vector2(inputX, inputZ).magnitude;

    bool isSprinting = Input.GetKey(KeyCode.LeftShift);
    float speedPercent = isSprinting ? inputMagnitude : inputMagnitude * 0.5f;

    animator?.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    animator?.SetBool("IsGrounded", IsGrounded);
    animator?.SetFloat("VerticalSpeed", velocity.y);
    } 
}
