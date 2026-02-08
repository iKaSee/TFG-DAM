using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Jump Refined (Nuevo)")]
    public float jumpInputBufferTime = 0.1f; // Tiempo que se guarda la pulsación antes de tocar suelo
    private float lastPressedJumpTime;
    private bool isJumping;
    private bool isJumpCut;
    [SerializeField] private float jumpCutGravityMult = 6f; // MODIFICADO: Subir a 6 para salto pesado
    [SerializeField] private float fallGravityMult = 4f;    // MODIFICADO: Subir a 4 para caída rápida
    private float defaultGravityScale;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float checkHeight = 0.3f;
    public float checkWidth = 1;
    public LayerMask groundLayer;

    [Header("Ajustes de Combate")]
    // MODIFICADO: Ahora es una penalización (resta velocidad) en lugar de un boost
    public float combatSpeedPenalty = 4f;
    private float defaultSpeed;

    [Header("Roll / Rodar")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.5f;
    public float rollCooldown = 0.8f;
    private bool isRolling = false;
    private bool canRoll = true;


    [Header("Crouch / Agacharse")]
    public float crouchSpeedMult = 0.5f; // Irá a la mitad de velocidad
    private bool isCrouching = false;


    [Header("Hitbox de Agachado")]
    private CapsuleCollider2D col; // Usamos CapsuleCollider2D
    private Vector2 originalSize;
    private Vector2 originalOffset;
    [SerializeField] private float crouchSizeMultiplier = 0.6f; // El colisionador medirá el 60% al agacharse

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerCombat combat;
    public bool isGrounded;
    private float moveInput;
    private bool facingRight = true;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
        defaultSpeed = moveSpeed;
        defaultGravityScale = rb.gravityScale; // Guardamos la gravedad inicial


        col = GetComponent<CapsuleCollider2D>();
        originalSize = col.size;
        originalOffset = col.offset;
    }

    void Update()
    {

        if (isRolling) return;


        // Detectar si pulsamos control para agacharnos, pero solo si estamos en el suelo
        if (isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else if (!Input.GetKey(KeyCode.LeftControl))
        {
            // Lanzamos un pequeño rayo o círculo hacia arriba para ver si hay techo
            bool hayTecho = Physics2D.OverlapCircle(transform.position + Vector3.up * 1f, 0.2f, groundLayer);

            if (!hayTecho)
            {
                isCrouching = false;
            }
            // Si hay techo, isCrouching se queda en true aunque sueltes la tecla
        }

        anim.SetBool("isCrouching", isCrouching);

        HandleHitbox();

        // MODIFICADO: Eliminado el bloqueo de Vector2.zero para que Jotem se mueva mientras ataca
        if (combat != null && combat.isAttacking)
        {
            // Ya no frenamos a cero, dejamos que UpdateAnimator siga fluyendo
            UpdateAnimator();
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        // 'moveInput' es la variable donde guardas el Input.GetAxisRaw("Horizontal")
        bool moviendose = Mathf.Abs(moveInput) > 0.01f;
        anim.SetBool("isMoving", moviendose);

        #region JUMP LOGIC
        // Timers
        lastPressedJumpTime -= Time.deltaTime;

        if (isGrounded)
        {
            isJumping = false;
            isJumpCut = false;
        }

        // DETECCIÓN DE SALTO (AQUÍ ESTABA EL ERROR)
        if (Input.GetButtonDown("Jump") && !isCrouching)
        {
            lastPressedJumpTime = jumpInputBufferTime;
        }

        // Salto variable
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.linearVelocity.y > 0.1f && isJumping)
            {
                isJumpCut = true;
                // Frenazo manual de velocidad para que el salto corto sea real
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        // Ejecución del Salto (Solo si hay buffer de tiempo y estamos en el suelo)
        if (lastPressedJumpTime > 0f && isGrounded && !isJumping)
        {
            Jump();
        }

        // Ajuste de Gravedad dinámico
        ModifyGravity();
        #endregion

        // Girar personaje (Tu método original)
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();


        // Bloqueamos inputs si está rodando


        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll && isGrounded)
        {
            StartCoroutine(ExecuteRoll());
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        // MODIFICADO: Eliminada la condición de ataque para permitir movimiento fluido
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(checkWidth, checkHeight), 0f, groundLayer);

        // Aplicamos movimiento manteniendo la Y del Rigidbody
        // Corregido: Ahora solo aplicamos la velocidad una vez calculando si está agachado
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMult : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (isRolling) return; // No saltar mientras se rueda

        isJumping = true;
        isJumpCut = false;

        // Reset de velocidad vertical para un salto consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        // Aplicamos el salto usando fuerza de impulso como en el script de Dawnosaur
        float force = jumpForce;
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        lastPressedJumpTime = 0;
    }

    private void ModifyGravity()
    {
        // CASO 1: Estamos cayendo (Velocidad negativa)
        if (rb.linearVelocity.y < -0.1f)
        {
            rb.gravityScale = defaultGravityScale * fallGravityMult;
        }
        // CASO 2: Estamos subiendo pero HEMOS SOLTADO el botón (Salto corto)
        else if (rb.linearVelocity.y > 0.1f && isJumpCut)
        {
            // Aplicamos una gravedad muy alta para que deje de subir inmediatamente
            rb.gravityScale = defaultGravityScale * jumpCutGravityMult;
        }
        // CASO 3: Subida normal (botón pulsado) o estamos en el suelo
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }
    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            groundCheck.position,
            new Vector3(checkWidth, checkHeight, 0f)
        );
    }

    void UpdateAnimator()
    {
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(moveInput));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        // IMPORTANTE: Asegúrate de que el parámetro en el Animator se llame exactamente "isCrouching"
        anim.SetBool("isCrouching", isCrouching);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private IEnumerator ExecuteRoll()
    {
        canRoll = false;
        isRolling = true;

        // Activamos la animación
        anim.SetTrigger("Roll");

        // Aplicamos velocidad constante hacia donde mira
        rb.linearVelocity = new Vector2((facingRight ? 1 : -1) * rollSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(rollDuration);

        isRolling = false;

        // Cooldown para no spamear el roll
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    private void HandleHitbox()
    {
        if (isCrouching)
        {
            // Reducimos el tamaño en Y
            col.size = new Vector2(originalSize.x, originalSize.y * crouchSizeMultiplier);

            // Bajamos el centro (Offset) para que la base del colisionador siga en los pies
            float diferencia = (originalSize.y - col.size.y) / 2f;
            col.offset = new Vector2(originalOffset.x, originalOffset.y - diferencia);
        }
        else
        {
            // Restauramos valores originales
            col.size = originalSize;
            col.offset = originalOffset;
        }
    }
}