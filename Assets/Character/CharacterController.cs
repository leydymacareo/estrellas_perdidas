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

    private bool recentlyExitedLadder = false;


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
        if (isOnLadder) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal < 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // izquierda
        }
        else if (horizontal > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f); // derecha
        }
        else if (vertical > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // frente
        }
        else if (vertical < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // atrás
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

        // Activamos el cooldown
        recentlyExitedLadder = true;
        Invoke(nameof(ResetLadderCooldown), 0.4f);  // 0.4 segundos de margen
    }

        void ResetLadderCooldown()
        {
            recentlyExitedLadder = false;
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
        transform.position = snapPoint.position;


        // Girar hacia la escalera
        transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Mira al frente siempre


        animator.SetBool("isOnLadder", true);
        Debug.Log("🎯 Entró a la escalera correctamente alineado al SnapPoint");
    }

    public void Rebotar()
    {
        // Rebote más suave que un salto normal
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 1f;
        animator?.SetBool("isJumping", true); // activa animación de salto si quieres
    }

    public void TocarLadderBottom(Collider trigger)
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (!isOnLadder && verticalInput > 0f)
        {
            Debug.Log("⬆ Entrando a la escalera desde abajo");
            EnterLadderFrom(trigger, false);
        }
        else if (isOnLadder && verticalInput < 0f)
        {
            Debug.Log("⬇ Saliendo de la escalera hacia abajo");
            ExitLadder();
        }
    }


    public void TocarLadderTop(Collider trigger)
    {
        float verticalInput = Input.GetAxis("Vertical");

        // 🚫 No permitimos volver a entrar desde arriba bajo ninguna circunstancia
        if (isOnLadder && verticalInput > 0f)
        {
            Debug.Log("⬆ Saliendo de la escalera hacia arriba");
            ExitLadder();
        }
    }

    public void EnterLadderFrom(Collider trigger, bool desdeArriba)
    {
        EnterLadder(trigger, desdeArriba);
    }

}
