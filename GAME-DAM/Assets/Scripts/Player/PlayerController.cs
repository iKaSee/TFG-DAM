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
    public bool invulnerable = false;


    [Header("Crouch / Agacharse")]
    public float crouchSpeedMult = 0.5f; // Irá a la mitad de velocidad
    private bool isCrouching = false;


    [Header("Hitbox de Agachado")]
    private CapsuleCollider2D col; // Usamos CapsuleCollider2D
    private Vector2 originalSize;
    private Vector2 originalOffset;
    [SerializeField] private float crouchSizeMultiplier = 0.6f; // El colisionador medirá el 60% al agacharse


    [Header("Run VFX")]
    public GameObject prefabPolvo; // Aquí arrastraremos el Prefab 
    public Transform puntoPies;    // Aquí arrastraremos el objeto PosicionPies

    [Header("Landing VFX")]
    public GameObject landingPrefab; //  Aquí arrastraremos el Prefab 
    private bool wasInAir; // Para saber si venimos de una caída

    [Header("Detección de Bordes")]
    public Transform edgeCheckPared; // Un punto a la altura del pecho
    public Transform edgeCheckVacio; // Un punto un poco por encima de la cabeza
    public float checkDistancia = 0.5f;
    private bool canGrabEdge = true; // Cambiado a true por defecto para que funcione
    private bool isGrabbingEdge = false;

    [Header("Escalado de Borde")]
    public Vector2 offsetFinalEscalado; // Cuánto se desplaza al terminar de subir
    private bool isClimbing = false;

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
        // CORREGIDO: "isClabbing" era una errata, es "isClimbing"
        if (isClimbing) return; // Si está escalando, no puede hacer nada más

        if (isGrabbingEdge)
        {
            // Opción A: SUBIR AL BORDE (Espacio / Jump)
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(ClimbEdge());
            }

            // Opción B: SOLTARSE (S o Abajo)
            if (Input.GetAxisRaw("Vertical") < -0.1f)
            {
                isGrabbingEdge = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                anim.SetBool("isEdgeGrabbing", false);
                StartCoroutine(TemporaryDisableEdgeGrab());
            }
            return; // Bloquea el resto del movimiento
        }

        CheckForEdge();

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

        if (!wasInAir && !isGrounded)
        {
            // Si no estábamos en el aire pero ahora isGrounded es false, es que acabamos de saltar o caer
            wasInAir = true;
        }

        if (wasInAir && isGrounded)
        {
            // ¡MOMENTO MÁGICO!: Estábamos en el aire y acabamos de tocar suelo
            CrearEfectoAterrizaje();
            wasInAir = false;
        }
    }

    void FixedUpdate()
    {
        if (isGrabbingEdge || isClimbing) return;

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
        if (isGrabbingEdge || isClimbing)
        {
            rb.gravityScale = 0;
            return;
        }

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

        if (edgeCheckPared != null && edgeCheckVacio != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(edgeCheckPared.position, transform.right * transform.localScale.x * checkDistancia);
            Gizmos.DrawRay(edgeCheckVacio.position, transform.right * transform.localScale.x * checkDistancia);
        }
    }

    void UpdateAnimator()
    {
        // Si estamos agarrados, no dejamos que la velocidad vertical o el suelo 
        // cambien la animación de agarre
        // Si está escalando, NO TOCAMOS NADA. El Animator se queda bloqueado en Climb.
        if (isClimbing) return;

        if (isGrabbingEdge)
        {
            anim.SetBool("isEdgeGrabbing", true);
            return;
        }

        anim.SetFloat("HorizontalSpeed", Mathf.Abs(moveInput));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        // Aseguramos que si no estamos agarrados, el bool del animator sea false
        anim.SetBool("isEdgeGrabbing", false);
        anim.SetBool("isClimbing", false);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void CheckForEdge()
    {
        // Solo intentamos agarrarnos si estamos cayendo o saltando (en el aire)
        if (!isGrounded && !isGrabbingEdge && canGrabEdge)
        {
            // Lanzamos un rayo desde el pecho
            bool tocaPared = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);
            // Lanzamos un rayo desde arriba de la cabeza
            bool tocaArriba = Physics2D.Raycast(edgeCheckVacio.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);

            // Si el de abajo toca pared pero el de arriba NO, hemos encontrado un borde
            if (tocaPared && !tocaArriba && rb.linearVelocity.y < 0)
            {
                SetEdgeGrab();
            }
        }
    }

    void SetEdgeGrab()
    {
        isGrabbingEdge = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // --- AJUSTE DE POSICIÓN ---
        RaycastHit2D hit = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);

        if (hit.collider != null)
        {
            float offsetHorizontal = 0.65f;
            float offsetVertical = -0.5f;
            Vector2 nuevaPos = new Vector2(hit.point.x - (offsetHorizontal * transform.localScale.x), transform.position.y + offsetVertical);
            transform.position = nuevaPos;
        }

        anim.SetBool("isEdgeGrabbing", true);
        anim.Play("Edge_Grab");
    }

    private IEnumerator ClimbEdge()
    {
        isClimbing = true;
        isGrabbingEdge = false;

        // Avisamos al Animator que empiece la subida
        anim.SetBool("isClimbing", true);
        anim.SetBool("isEdgeGrabbing", false);

        // Reproducimos la animación
        anim.Play("Edge_Grab_Climb");

        // Esperamos a que la animación termine visualmente
        yield return new WaitForSeconds(0.6f);

        // Posicionamiento final
        Vector2 posFinal = new Vector2(
            transform.position.x + (offsetFinalEscalado.x * (facingRight ? 1 : -1)),
            transform.position.y + offsetFinalEscalado.y
        );
        transform.position = posFinal;

        // Restaurar físicas
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Avisamos que ya terminó de escalar
        isClimbing = false;
        anim.SetBool("isClimbing", false);
    }

    private IEnumerator TemporaryDisableEdgeGrab()
    {
        canGrabEdge = false;
        yield return new WaitForSeconds(0.3f);
        canGrabEdge = true;
    }

    private IEnumerator ExecuteRoll()
    {
        canRoll = false;
        isRolling = true;
        invulnerable = true;

        int playerLayer = gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer("Enemies");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        anim.SetTrigger("Roll");

        float rollDir = facingRight ? 1 : -1;
        float currentGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float timer = 0;
        while (timer < rollDuration)
        {
            rb.linearVelocity = new Vector2(rollDir * rollSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = currentGravity;
        invulnerable = false;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        isRolling = false;

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    public void CrearPolvo()
    {
        Instantiate(prefabPolvo, puntoPies.position, Quaternion.identity);
    }

    public void CrearEfectoAterrizaje()
    {
        if (landingPrefab != null && puntoPies != null)
        {
            Instantiate(landingPrefab, puntoPies.position, Quaternion.identity);
        }
    }

    private void HandleHitbox()
    {
        if (isCrouching)
        {
            col.size = new Vector2(originalSize.x, originalSize.y * crouchSizeMultiplier);
            float diferencia = (originalSize.y - col.size.y) / 2f;
            col.offset = new Vector2(originalOffset.x, originalOffset.y - diferencia);
        }
        else
        {
            col.size = originalSize;
            col.offset = originalOffset;
        }
    }
}