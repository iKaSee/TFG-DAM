using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Cinemática Inicial")]
    public bool empezarTumbado = false;
    private bool controlsEnabled = true;

    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Jump Refined (Nuevo)")]
    public float jumpInputBufferTime = 0.1f;
    private float lastPressedJumpTime;
    private bool isJumping;
    private bool isJumpCut;
    [SerializeField] private float jumpCutGravityMult = 6f;
    [SerializeField] private float fallGravityMult = 6f;
    private float defaultGravityScale;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float checkHeight = 0.3f;
    public float checkWidth = 1;
    public LayerMask groundLayer;

    [Header("Ajustes de Combate")]
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
    public float crouchSpeedMult = 0.5f;
    private bool isCrouching = false;

    [Header("Hitbox de Agachado")]
    private CapsuleCollider2D col;
    private Vector2 originalSize;
    private Vector2 originalOffset;
    [SerializeField] private float crouchSizeMultiplier = 0.6f;

    [Header("Run VFX")]
    public GameObject prefabPolvo;
    public Transform puntoPies;

    [Header("Jump VFX (Artista)")]
    public GameObject jumpVFX;

    [Header("Landing VFX")]
    public GameObject landingPrefab;
    private bool wasInAir;

    [Header("Detección de Bordes")]
    public Transform edgeCheckPared;
    public Transform edgeCheckVacio;
    public float checkDistancia = 0.5f;
    private bool canGrabEdge = true;
    private bool isGrabbingEdge = false;

    [Header("Escalado de Borde")]
    public Vector2 offsetFinalEscalado;
    private bool isClimbing = false;

    [Header("Wall Movement (Nuevo)")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public float wallSlidingSpeed = 2f;
    private bool isWallSliding;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce = new Vector2(10f, 20f);
    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;



    [Header("Fricción de Aire")]
    public float airControlForce = 5f;

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
        defaultGravityScale = rb.gravityScale;

        col = GetComponent<CapsuleCollider2D>();
        originalSize = col.size;
        originalOffset = col.offset;
    }

    void Start()
    {
        if (empezarTumbado)
        {
            controlsEnabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Forzamos el estado de tumbado y cerramos la llave en el Animator
            anim.Play("Start_Lying");
            anim.SetBool("YaDespierto", false);
        }
    }

    IEnumerator Levantarse()
    {
        anim.speed = 1;
        anim.Play("Get_Up");

        // 1. Esperamos a que la animación de levantarse casi termine
        yield return new WaitForSeconds(1.1f); // Un pelín menos del tiempo total

        // 2. PREPARACIÓN: Forzamos los parámetros para que no pueda caer
        anim.SetBool("isGrounded", true);
        anim.SetFloat("VerticalVelocity", 0);
        anim.SetBool("YaDespierto", true);

        // 3. Forzamos el paso a Idle manualmente para saltarnos cualquier transición
        anim.Play("idle_Jotem");

        // 4. Activamos la física al final de todo
        rb.bodyType = RigidbodyType2D.Dynamic;
        controlsEnabled = true;
        empezarTumbado = false;
    }




    void Update()
    {

        if (!controlsEnabled)
        {
            // Mientras no estemos despiertos, bloqueamos el Animator para que no se mueva
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Start_Lying"))
            {
                anim.speed = 0;
            }

            if (Input.anyKeyDown)
            {
                StartCoroutine(Levantarse());
            }
            return;
        }
        if (isClimbing) return;

        // --- CORRECCIÓN DE GIRO ---
        // Solo giramos si NO estamos deslizando y NO estamos en el impulso del salto de pared
        if (!isWallSliding && wallJumpCounter <= 0 && !isRolling)
        {
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll && isGrounded && !isWallSliding)
        {
            StartCoroutine(ExecuteRoll());
        }

        if (isGrabbingEdge)
        {
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(ClimbEdge());
            }

            if (Input.GetAxisRaw("Vertical") < -0.1f)
            {
                isGrabbingEdge = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                anim.SetBool("isEdgeGrabbing", false);
                StartCoroutine(TemporaryDisableEdgeGrab());
            }
            return;
        }

        CheckForEdge();
        WallSlide(); // Lógica de deslizamiento corregida
        WallJump();  // Lógica de salto en pared

        if (isRolling || wallJumpCounter > 0) return;

        if (isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else if (!Input.GetKey(KeyCode.LeftControl))
        {
            bool hayTecho = Physics2D.OverlapCircle(transform.position + Vector3.up * 1f, 0.2f, groundLayer);
            if (!hayTecho) isCrouching = false;
        }

        anim.SetBool("isCrouching", isCrouching);
        HandleHitbox();

        if (combat != null && combat.isAttacking)
        {
            UpdateAnimator();
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        bool moviendose = Mathf.Abs(moveInput) > 0.01f;
        anim.SetBool("isMoving", moviendose);

        #region JUMP LOGIC

        if (isGrounded)
        {
            isJumping = false;
            isJumpCut = false;
            wallJumpCounter = 0;

            // --- ADAPTACIÓN AL ARTISTA ---
            // Cuando Jotem toca el suelo, apagamos el booleano de salto
            anim.SetBool("Jump", false);
        }

        if (Input.GetButtonDown("Jump") && !isCrouching)
        {
            lastPressedJumpTime = jumpInputBufferTime;
        }

        // --- CAMBIO CLAVE AQUÍ ---
        if (lastPressedJumpTime > 0f)
        {
            // 1. Si estamos en la pared, PRIORIDAD al WallJump
            if (wallJumpCounter > 0)
            {
                ExecuteWallJump(); // Llamamos a una función limpia
            }
            // 2. Si NO estamos en la pared pero SÍ en el suelo, salto normal
            else if (isGrounded && !isJumping)
            {
                Jump();
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            // Si soltamos el botón y aún estamos subiendo...
            if (rb.linearVelocity.y > 0 && isJumping)
            {
                isJumpCut = true;
                // Aplicamos un frenazo inmediato hacia abajo para que el salto sea corto
                // Esto NO cambia tu fórmula de gravedad, solo le da un "empujón" inicial
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.4f);
            }
        }

        ModifyGravity();


        #endregion

        UpdateAnimator();

        if (!wasInAir && !isGrounded) wasInAir = true;
        if (wasInAir && isGrounded)
        {
            CrearEfectoAterrizaje();
            wasInAir = false;
        }
    }

    



    void FixedUpdate()
    {
        if (isGrabbingEdge || isClimbing || wallJumpCounter > 0 || isRolling) return;

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(checkWidth, checkHeight), 0f, groundLayer);

        float velocidadTarget;
        if (isWallSliding)
        {
            velocidadTarget = 0;
        }
        else
        {
            float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMult : moveSpeed;
            velocidadTarget = moveInput * currentSpeed;
        }

        // --- CAMBIO PARA LA FRICCIÓN DE AIRE ---
        if (isGrounded)
        {
            // En el suelo, el movimiento es instantáneo (como hasta ahora)
            rb.linearVelocity = new Vector2(velocidadTarget, rb.linearVelocity.y);
        }
        else
        {
            // En el aire, usamos Lerp para que la velocidad cambie poco a poco
            // Esto crea esa sensación de "inercia" o fricción de aire
            float velocidadSuave = Mathf.Lerp(rb.linearVelocity.x, velocidadTarget, Time.fixedDeltaTime * airControlForce);
            rb.linearVelocity = new Vector2(velocidadSuave, rb.linearVelocity.y);
        }
    }

    private void WallSlide()
    {
        // --- SOLUCIÓN DETECCIÓN (PUNTO 4) ---
        // Aumentamos el ancho del rayo solo para la detección 
        // y usamos una pequeña compensación (0.1f) para que no detecte "dentro" del cuerpo
        Vector2 rayOrigin = wallCheck.position;
        float laserRange = wallCheckDistance + 0.2f;

        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, laserRange, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, laserRange, groundLayer);

        bool wallRight = hitRight.collider != null;
        bool wallLeft = hitLeft.collider != null;
        bool isTouchingWall = wallRight || wallLeft;

        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0 && !isGrabbingEdge)
        {
            isWallSliding = true;

            RaycastHit2D hitActivo = wallRight ? hitRight : hitLeft;
            float direccionPared = wallRight ? 1 : -1;

            float radioCuerpo = col.size.x / 2f;
            float posicionXFija = hitActivo.point.x - (radioCuerpo * direccionPared);

            transform.position = new Vector2(posicionXFija, transform.position.y);
            rb.linearVelocity = new Vector2(0, Mathf.Max(rb.linearVelocity.y, -wallSlidingSpeed));

            if (wallRight && facingRight) Flip();
            else if (wallLeft && !facingRight) Flip();
        }
        else
        {
            isWallSliding = false;
        }

        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpCounter = wallJumpTime;
        }
        else if (!isGrounded) // Solo restamos si NO estamos en el suelo
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0 && !isGrounded) // Añadimos !isGrounded por seguridad
        {
            isJumping = true;
            isJumpCut = false;

            anim.SetTrigger("WallJumpTrigger");

            rb.linearVelocity = Vector2.zero;
            float jumpDir = facingRight ? 1 : -1;

            // Aplicamos la fuerza
            rb.AddForce(new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y), ForceMode2D.Impulse);

            wallJumpCounter = 0;
            // Reiniciamos el buffer de salto normal para que no salte otra vez al caer
            lastPressedJumpTime = 0;
        }
    }

    private bool isWallJumping;

    private void ExecuteWallJump()
    {
        isWallJumping = true; // Bloqueamos otras animaciones un momento
        isJumping = true;
        isJumpCut = false;

        // 1. Limpiamos cualquier rastro de otros saltos
        anim.ResetTrigger("WallJumpTrigger");

        // 2. Disparamos el Trigger (asegúrate de que se llame exactamente así en Unity)
        anim.SetTrigger("WallJumpTrigger");

        // 3. Física (Punto 2 y 4)
        rb.linearVelocity = Vector2.zero;
        float jumpDir = facingRight ? 1 : -1;
        rb.AddForce(new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y), ForceMode2D.Impulse);

        wallJumpCounter = 0;
        lastPressedJumpTime = 0;

        // 4. Desbloqueamos después de un pequeño tiempo (lo que dura el impulso)
        Invoke("ResetWallJumpAnim", 0.3f);
    }

    private void ResetWallJumpAnim() { isWallJumping = false; }

    private void Jump()
    {
        if (isRolling) return;

        isJumping = true;
        isJumpCut = false;

        // Aplicamos la fuerza igual que él
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // --- ADAPTACIÓN AL ARTISTA ---
        // Activamos el booleano que sus animaciones necesitan
        anim.SetBool("Jump", true);

        if (jumpVFX != null && puntoPies != null)
        {
            Instantiate(jumpVFX, puntoPies.position, Quaternion.identity);
        }

        lastPressedJumpTime = 0;
    }

    private void ModifyGravity()
    {
        if (isGrabbingEdge || isClimbing || isWallSliding)
        {
            rb.gravityScale = isWallSliding ? defaultGravityScale : 0;
            return;
        }

        if (rb.linearVelocity.y < -0.1f)
        {
            // Tu fórmula de caída normal que te gusta
            rb.gravityScale = defaultGravityScale * fallGravityMult;
        }
        else if (rb.linearVelocity.y > 0.1f && isJumpCut)
        {
            // Si ha soltado el botón, aplicamos más gravedad para que suba menos
            rb.gravityScale = defaultGravityScale * jumpCutGravityMult;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(checkWidth, checkHeight, 0f));
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            // Dibujamos rayos a ambos lados en el editor
            Gizmos.DrawRay(wallCheck.position, Vector2.right * wallCheckDistance);
            Gizmos.DrawRay(wallCheck.position, Vector2.left * wallCheckDistance);
        }
    }

    void UpdateAnimator()
    {
        // --- BLOQUEO TOTAL DURANTE EL INICIO (Punto clave) ---
        if (!controlsEnabled && empezarTumbado)
        {
            // Forzamos a que el Animator se quede en el estado de tumbado
            // y no lea nada de lo que viene abajo (isGrounded, Velocidad, etc.)
            return;
        }

        if (isClimbing || isGrabbingEdge) return;

        // --- PRIORIDAD ABSOLUTA (Punto 3) ---
        // Si estamos haciendo el salto de pared, no dejamos que entre ninguna otra animación
        if (isWallJumping) return;

        anim.SetBool("isWallSliding", isWallSliding);

        if (isWallSliding)
        {
            anim.SetBool("isGrounded", false);
            anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
            return;
        }


        anim.SetBool("isWallSliding", false);
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(moveInput));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isMoving", Mathf.Abs(moveInput) > 0.01f);
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
        if (!isGrounded && !isGrabbingEdge && canGrabEdge)
        {
            bool tocaPared = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);
            bool tocaArriba = Physics2D.Raycast(edgeCheckVacio.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);

            if (tocaPared && !tocaArriba && rb.linearVelocity.y < 0)
            {
                SetEdgeGrab();
            }
        }
    }

    void SetEdgeGrab()
    {
        isGrabbingEdge = true;
        isWallSliding = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        RaycastHit2D hit = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, groundLayer);

        if (hit.collider != null)
        {
            float offsetHorizontal = 0.65f;
            float offsetVertical = -0.55f;
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
        anim.SetBool("isClimbing", true);
        anim.SetBool("isEdgeGrabbing", false);
        anim.Play("Edge_Grab_Climb");
        yield return new WaitForSeconds(0.6f);
        Vector2 posFinal = new Vector2(transform.position.x + (offsetFinalEscalado.x * (facingRight ? 1 : -1)), transform.position.y + offsetFinalEscalado.y);
        transform.position = posFinal;
        rb.bodyType = RigidbodyType2D.Dynamic;
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

    public void CrearPolvo() { if (prefabPolvo != null) Instantiate(prefabPolvo, puntoPies.position, Quaternion.identity); }

    public void CrearEfectoAterrizaje() { if (landingPrefab != null) Instantiate(landingPrefab, puntoPies.position, Quaternion.identity); }

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