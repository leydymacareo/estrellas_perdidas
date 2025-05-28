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

            animator.SetBool("isOnLadder", true);
            animator.SetFloat("ClimbSpeed", Mathf.Abs(verticalInput)); // 0 = quieto, 1 = subiendo

            return; // salimos del resto del movimiento

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

            Vector3 centerXZ = new Vector3(other.bounds.center.x, transform.position.y, other.bounds.center.z);
            transform.position = centerXZ;

            // Girar hacia la escalera
            Vector3 lookDirection = -other.transform.forward;
            lookDirection.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDirection);

            animator.SetBool("isOnLadder", true);
        }


    }


    void OnTriggerExit(Collider other)
    {

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

    void ExitLadder()
    {
        isOnLadder = false;
        velocity = Vector3.zero;
        animator.SetBool("isOnLadder", false);

        // Empuje hacia adelante y un poco hacia arriba para salir de la escalera
        Vector3 offset = transform.forward * 0.6f + Vector3.up * 0.3f;
        characterController.Move(offset);

        Debug.Log("ExitLadder ejecutado");
    }

    void OnTriggerStay(Collider other)
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (other.CompareTag("LadderBottom"))
        {
            Debug.Log("🧩 Dentro del LadderBottom");

            if (!isOnLadder && verticalInput > 0f)
            {
                Debug.Log("⬆ Entrando a la escalera desde abajo");
                EnterLadder(other, false); // Desde abajo
            }
            else if (isOnLadder && verticalInput < 0f)
            {
                Debug.Log("⬇ Saliendo de la escalera hacia abajo");
                ExitLadder();
            }
        }

        if (other.CompareTag("LadderTop"))
        {
            Debug.Log(isOnLadder);
            Debug.Log(verticalInput);
            Debug.Log("🧩 Dentro del LadderTop");

            if (!isOnLadder && verticalInput < 0f)
            {
                Debug.Log("⬇ Entrando a la escalera desde arriba");
                EnterLadder(other, true); // Desde arriba
            }
            else if (isOnLadder && verticalInput > 0f)
            {
                Debug.Log("⬆ Saliendo de la escalera hacia arriba");
                ExitLadder();
            }
        }

    }

    void EnterLadder(Collider trigger, bool desdeArriba)
    {
        Debug.Log("EnterLadder ejecutado");

        isOnLadder = true;
        velocity = Vector3.zero;

        Transform snapPoint = trigger.transform.parent.Find("LadderSnapPoint");
        if (snapPoint == null)
        {
            Debug.LogWarning("❌ No se encontró LadderSnapPoint en la escalera");
            return;
        }

        // Si entra desde arriba, lo bajamos un poco más para "encajarlo" dentro de la escalera
        float snapY = desdeArriba ? snapPoint.position.y - 0.1f : transform.position.y;

        Vector3 snapPosition = new Vector3(snapPoint.position.x, snapY, snapPoint.position.z);
        transform.position = snapPosition;

        // Girar hacia la escalera
        Vector3 lookDirection = -trigger.transform.parent.forward;
        lookDirection.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        animator.SetBool("isOnLadder", true);
        Debug.Log("🎯 Entró a la escalera correctamente alineado al SnapPoint");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Mancha"))
        {
            ManchaEnemiga mancha = hit.collider.GetComponent<ManchaEnemiga>();
            if (mancha != null)
            {
                mancha.SerTocado(transform); // le pasamos la posición del jugador
            }
        }
    }


}
