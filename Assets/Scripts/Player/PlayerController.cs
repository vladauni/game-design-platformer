using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Рух")]
    [SerializeField, Range(1f, 20f)] private float maxSpeed = 8f;
    [SerializeField, Range(5f, 200f)] private float acceleration = 70f;
    [SerializeField, Range(5f, 200f)] private float deceleration = 90f;

    private Rigidbody2D body;
    private PlayerControls controls;
    private float moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
    }

    // Без Enable введення не приходить — і помилки при цьому не буде
    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();
    private void OnDestroy() => controls.Dispose();

    private void Update()
    {
        isGrounded = CheckGround();

        // Поповнюємо coyote лише коли стоїмо, а не летимо вгору — див. крок 6
        bool canRefill = isGrounded && body.linearVelocity.y <= 0.01f;
        coyoteCounter = canRefill ? coyoteTime : coyoteCounter - Time.deltaTime;

        // Натиск не виконуємо одразу, а ЗАПАМ'ЯТОВУЄМО в таймер
        jumpBufferCounter = controls.Player.Jump.WasPressedThisFrame()
            ? jumpBufferTime
            : jumpBufferCounter - Time.deltaTime;

        // Стрибок відбувається, коли активні ОБИДВА таймери
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>().x;
        
        
    }

    private void FixedUpdate()
    {
        float target = moveInput * maxSpeed;

        // Гальмуємо ШВИДШЕ, ніж розганяємось — саме це дає відчуття контролю
        float rate = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;

        float x = Mathf.MoveTowards(body.linearVelocity.x, target,
                                    rate * Time.fixedDeltaTime);
        body.linearVelocity = new Vector2(x, body.linearVelocity.y);
    }

    [Header("Земля")]
    [SerializeField] private Transform groundCheck;
    [SerializeField, Range(0.05f, 0.5f)] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;

    // Коло перекриває всю ширину ніг, а не одну точку
    private bool CheckGround() =>
        Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    // Без візуалізації датчик налагоджувати неможливо
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Application.isPlaying && isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    [Header("Стрибок")]
    [SerializeField, Range(0.5f, 8f)] private float jumpHeight = 3f;

    // Базовий масштаб гравітації. На кроці 8 ми будемо його множити,
    // але розрахунок висоти МУСИТЬ спиратися саме на це число
    [SerializeField, Range(0.5f, 8f)] private float baseGravity = 3f;

    private float JumpVelocity =>
        Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * baseGravity * jumpHeight);

    private void Jump()
    {
        // Фіксуємо масштаб ДО імпульсу: інакше злетимо з чужою гравітацією
        body.gravityScale = baseGravity;
        body.linearVelocity = new Vector2(body.linearVelocity.x, JumpVelocity);
    }

    
        [Header("Прийоми тюнінгу")]
    [SerializeField, Range(0f, 0.4f)] private float coyoteTime = 0.12f;
    private float coyoteCounter;

    [SerializeField, Range(0f, 0.4f)] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;


}
