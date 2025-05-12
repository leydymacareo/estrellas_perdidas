using UnityEngine;

[RequireComponent(typeof(UnityEngine.CharacterController))]
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

    private UnityEngine.CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;

    private const float gravity = -20f;

    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public float CurrentYaw => yaw;

    void Start()
    {
        characterController = GetComponent<UnityEngine.CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        //UpdateAnimator();
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
            // animator?.SetBool("isJumping", false);
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

    /* void UpdateAnimator()
    {
        float speedPercent = IsMoving ? (currentSpeed == SprintSpeed ? 1f : 0.5f) : 0f;
        animator?.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", velocity.y);
    }*/
}
