using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Dash")]
    public float dashSpeed = 15f; // Ajustada para que se aprecie la animación
    public float dashDuration = 0.35f; // Ajustada para que coincida con el sprite
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerCombat combat;
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        // Si estamos dasheando, no procesamos más inputs
        if (isDashing) return;

        // Bloqueo de movimiento por ataque (Soulslike)
        if (combat != null && combat.isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(); // Para que pase a Idle si estaba corriendo
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        // Lógica de Coyote Time
        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        // Salto
        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }

        // Botón del Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(PerformDash());
        }

        // Girar personaje (Tu método original)
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        // Si estamos dasheando o atacando, no aplicamos movimiento de caminata
        if (isDashing || (combat != null && combat.isAttacking)) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Aplicamos la velocidad
        rb.linearVelocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, 0f);
        anim.SetTrigger("Dash");

        // Esperamos la duración exacta
        yield return new WaitForSeconds(dashDuration);

        // --- EL CORTE MAESTRO ---
        rb.linearVelocity = Vector2.zero; // Frenazo total
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Esperar al cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void UpdateAnimator()
    {
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(moveInput));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}