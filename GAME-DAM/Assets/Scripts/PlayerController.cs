using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;
    private Animator animator;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int extraJumpsValue;

    // ===================================
    // ⚔️ NUEVAS VARIABLES PARA EL ATAQUE
    // ===================================
    public GameObject attackHitbox; // Asigna aquí el objeto AttackHitbox (hijo del Jugador)
    private bool isAttacking = false; // Controla si el jugador ya está en la animación de ataque

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // --- LÍNEA NUEVA PARA EL SALTO ---
        // Le decimos al animator: "¿Estoy saltando? Sí, si NO estoy en el suelo (!isGrounded)"
        animator.SetBool("IsJumping", !isGrounded);
        // ---------------------------------

        //NUEVO: Avisamos de la velocidad vertical (Para saber si sube o baja)
        animator.SetFloat("vSpeed", rb.linearVelocity.y);


        moveInput = Input.GetAxis("Horizontal");

        // 🛑 NUEVO: Bloquear el movimiento horizontal si estamos atacando
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        }
        else
        {
            // Opcional: Reducir velocidad horizontal mientras ataca, o mantenerla en 0
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // 3. NUEVO: ENVIAR VELOCIDAD AL ANIMATOR
        // Usamos Mathf.Abs (Valor Absoluto) porque moveInput va de -1 a 1.
        // Al Animator solo le importa la velocidad positiva (0 a 1).
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (facingRight == false && moveInput > 0)
        {
            flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            flip();
        }
    }


    void Update()
    {
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            rb.linearVelocity = Vector2.up * (jumpForce - 2);
        }

        // ===================================
        // ⚔️ NUEVA LÓGICA DE ATAQUE (Input)
        // ===================================
        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && isGrounded) // Usamos la tecla X como ejemplo
        {
            StartAttack();
        }
    }

    // ===================================
    // ⚔️ NUEVOS MÉTODOS DE ATAQUE
    // ===================================

    // Método que se llama al pulsar la tecla de ataque
    private void StartAttack()
    {
        isAttacking = true;
        // Inicia la animación de ataque, usando el Trigger que configuraste
        animator.SetTrigger("Attack");
    }

    // MÉTODOS LLAMADOS POR ANIMATION EVENTS

    // 1. Llamado en el frame de impacto de la animación
    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }
    }

    // 2. Llamado después del frame de impacto para finalizar el ataque
    public void DisableHitboxAndEndAttack()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
        isAttacking = false; // Permite que el jugador pueda moverse y atacar de nuevo
    }

    // ===================================
    // ⚔️ FIN DE MÉTODOS DE ATAQUE
    // ===================================

    void flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}