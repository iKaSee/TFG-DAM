using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Cinemática Inicial")]
    public bool empezarTumbado = false;
    private bool controlsEnabled = true;
    public bool enCinematica = false;
    public float jumpForce = 12f;
    
    // --- Candado para no repetir la animación al levantarse ---
    private bool seEstaLevantando = false; 
    // ----------------------------------------------------------------

    [Header("Estados Especiales")]
    public bool soloCaminar = false; // Si esto es true, Jotem entra en modo NPC/Cinemática


    [Header("Jump Refined ")]
    public float jumpInputBufferTime = 0.1f;
    private float lastPressedJumpTime;
    private bool isJumping;
    private bool isJumpCut;
    [SerializeField] private float jumpCutGravityMult = 6f;
    [SerializeField] private float fallGravityMult = 6f;
    private float defaultGravityScale;

    // --- Coyote Time y Límite de Caída ---
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public float maxFallSpeed = 25f; 
    // ---------------------------------------------

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float checkHeight = 0.3f;
    public float checkWidth = 1;
    public LayerMask groundLayer;
    public LayerMask wallLayer; 

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

    [Header("Wall Movement")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public float wallSlidingSpeed = 13f; 
    private bool isWallSliding;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce = new Vector2(10f, 20f);
    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    
    private float blockWallSlideTimer; 

    [Header("Fricción de Aire")]
    public float airControlForce = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerCombat combat;
    public bool isGrounded;
    private float moveInput;
    private bool facingRight = true;

    void Start()
    {
        string nombreEscena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int indexCapaTutorial = anim.GetLayerIndex("Tutorial");

        if (nombreEscena == "Mina")
        {
            ForzarEstadoTumbado();
        }
        else if (indexCapaTutorial != -1)
        {
            anim.SetLayerWeight(indexCapaTutorial, 0f);
        }
    }



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
        
        defaultGravityScale = rb.gravityScale;
        col = GetComponent<CapsuleCollider2D>();
        originalSize = col.size;
        originalOffset = col.offset;

        controlsEnabled = true;
        empezarTumbado = false;
    }

    public void ForzarEstadoTumbado()
    {
        empezarTumbado = true;
        controlsEnabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;

        int indexCapaTutorial = anim.GetLayerIndex("Tutorial");
        if (indexCapaTutorial != -1)
        {
            anim.SetLayerWeight(indexCapaTutorial, 1f);
            anim.Play("Start_Lying", indexCapaTutorial);
        }
        
        anim.SetBool("YaDespierto", false);
        anim.speed = 0; 
    }

    IEnumerator Levantarse()
    {
        anim.speed = 1;
        int indexCapaTutorial = anim.GetLayerIndex("Tutorial");
        
        if (indexCapaTutorial != -1) anim.Play("Get_Up", indexCapaTutorial);
        else anim.Play("Get_Up");

        yield return new WaitForSeconds(1.1f);

        if (indexCapaTutorial != -1) anim.SetLayerWeight(indexCapaTutorial, 0f);
        
        anim.SetBool("isGrounded", true);
        anim.SetFloat("VerticalVelocity", 0);
        anim.SetBool("YaDespierto", true);
        anim.Play("idle_Jotem"); 

        rb.bodyType = RigidbodyType2D.Dynamic;
        controlsEnabled = true;
        empezarTumbado = false;
        
        seEstaLevantando = false; 
    }

    void Update()
    {
        if (enCinematica)
        {
            moveInput = 0f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            anim.SetFloat("HorizontalSpeed", 0f);
            anim.SetBool("isMoving", false);
            return; 
        }

        if (blockWallSlideTimer > 0) blockWallSlideTimer -= Time.deltaTime;
        // -----------------------------------------------------

        if (!controlsEnabled && empezarTumbado)
        {
            if (Input.anyKeyDown && !seEstaLevantando)
            {
                seEstaLevantando = true;
                StartCoroutine(Levantarse());
            }
            return; 
        }

        if (isClimbing) return;
        if (!isWallSliding && wallJumpCounter <= 0 && !isRolling)
        {
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll && isGrounded && !isWallSliding && !soloCaminar) 
        {
            StartCoroutine(ExecuteRoll());
        }
        if (isGrabbingEdge)
        {
            if (Input.GetButtonDown("Jump") && !isCrouching && !soloCaminar)
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
        WallSlide();
        WallJump();

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
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
            isJumpCut = false;
            wallJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && !isCrouching)
        {
            lastPressedJumpTime = jumpInputBufferTime;
        }
        else 
        {
            lastPressedJumpTime -= Time.deltaTime; 
        }

        if (lastPressedJumpTime > 0f)
        {
            if (wallJumpCounter > 0)
            {
                ExecuteWallJump();
            }
            else if (coyoteTimeCounter > 0f && !isJumping) 
            {
                Jump();
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (rb.linearVelocity.y > 0 && isJumping)
            {
                isJumpCut = true;
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

        LayerMask mascaraPisar = groundLayer | LayerMask.GetMask("Plataformas") | LayerMask.GetMask("Trap");
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(checkWidth, checkHeight), 0f, mascaraPisar);

        float velocidadTarget;

        if (isWallSliding)
        {
            velocidadTarget = 0;
        }
        else
        {
            float currentSpeed;

            if (soloCaminar)
            {
                currentSpeed = MoveSpeed * 0.5f; 
            }
            else
            {
                currentSpeed = isCrouching ? MoveSpeed * crouchSpeedMult : MoveSpeed;
            }

            velocidadTarget = moveInput * currentSpeed;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(velocidadTarget, rb.linearVelocity.y);
        }
        else
        {
            float velocidadSuave = Mathf.Lerp(rb.linearVelocity.x, velocidadTarget, Time.fixedDeltaTime * airControlForce);
            rb.linearVelocity = new Vector2(velocidadSuave, rb.linearVelocity.y);
        }
    }

    private void WallSlide()
    {
        if (blockWallSlideTimer > 0)
        {
            isWallSliding = false;
            anim.SetBool("isWallSliding", false);
            return;
        }
        // ----------------------------------------------------------------------------------

        Vector2 rayOrigin = wallCheck.position;
        float laserRange = wallCheckDistance + 0.2f;

        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, laserRange, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, laserRange, wallLayer);

        bool wallRight = hitRight.collider != null;
        bool wallLeft = hitLeft.collider != null;
        bool isTouchingWall = wallRight || wallLeft;

        if (isTouchingWall && !isGrounded && !isGrabbingEdge)
        {
            isWallSliding = true;

            RaycastHit2D hitActivo = wallRight ? hitRight : hitLeft;
            float direccionPared = wallRight ? 1 : -1;

            float radioCuerpo = col.size.x / 2f;
            float posicionXFija = hitActivo.point.x - (radioCuerpo * direccionPared);

            transform.position = new Vector2(posicionXFija, transform.position.y);
            
            float velocidadY = rb.linearVelocity.y;
            if (velocidadY > 0) velocidadY = 0;

            rb.linearVelocity = new Vector2(0, Mathf.Max(velocidadY, -wallSlidingSpeed));

            anim.SetBool("Jump", false);
            isJumping = false;

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
        else if (!isGrounded)
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0 && !isGrounded)
        {
            isJumping = true;
            isJumpCut = false;

            anim.SetTrigger("WallJumpTrigger");

            rb.linearVelocity = Vector2.zero;
            float jumpDir = facingRight ? 1 : -1;

            rb.AddForce(new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y), ForceMode2D.Impulse);

            wallJumpCounter = 0;
            lastPressedJumpTime = 0;

            blockWallSlideTimer = 0.2f;
        }
    }

    private bool isWallJumping;

    public float MoveSpeed { get => MoveSpeed1; set => MoveSpeed1 = value; }
    [field: Header("Movimiento")]
    public float MoveSpeed1 { get; set; } = 8f;

    private void ExecuteWallJump()
    {
        isWallJumping = true;
        isJumping = true;
        isJumpCut = false;

        anim.ResetTrigger("WallJumpTrigger");
        anim.SetTrigger("WallJumpTrigger");

        rb.linearVelocity = Vector2.zero;
        float jumpDir = facingRight ? 1 : -1;
        rb.AddForce(new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y), ForceMode2D.Impulse);

        wallJumpCounter = 0;
        lastPressedJumpTime = 0;
        
        blockWallSlideTimer = 0.2f;

        Invoke("ResetWallJumpAnim", 0.3f);
    }

    private void ResetWallJumpAnim() { isWallJumping = false; }

    private void Jump()
    {
        if (isRolling) return;

        isJumping = true;
        isJumpCut = false;
        
        // --- Agotamos el Coyote Time para no hacer saltos dobles en el aire ---
        coyoteTimeCounter = 0f;
        // -----------------------------------------------------------------------------

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
            rb.gravityScale = defaultGravityScale * fallGravityMult;
        }
        else if (rb.linearVelocity.y > 0.1f && isJumpCut)
        {
            rb.gravityScale = defaultGravityScale * jumpCutGravityMult;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }

        // --- Velocidad Terminal ---
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
        // ------------------------------------------------------------------------
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
            Gizmos.DrawRay(wallCheck.position, Vector2.right * wallCheckDistance);
            Gizmos.DrawRay(wallCheck.position, Vector2.left * wallCheckDistance);
        }
    }

    void UpdateAnimator()
    {
        if (!controlsEnabled && empezarTumbado) return;

        if (isClimbing || isGrabbingEdge) return;
        if (isWallJumping) return;

        anim.SetBool("isWallSliding", isWallSliding);

        if (isWallSliding)
        {
            anim.SetBool("isGrounded", false);
            anim.SetBool("isJumping", false);
            anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
            return;
        }




        anim.SetBool("isWallSliding", false);
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(moveInput));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        bool estaCorriendo = Mathf.Abs(moveInput) > 0.01f && !soloCaminar;
        anim.SetBool("isMoving", estaCorriendo);
        anim.SetBool("isWalking", soloCaminar);

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
            LayerMask capasAgarre = groundLayer | LayerMask.GetMask("Plataformas");

            bool tocaPared = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, capasAgarre);
            bool tocaArriba = Physics2D.Raycast(edgeCheckVacio.position, transform.right * transform.localScale.x, checkDistancia, capasAgarre);

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

        LayerMask capasAgarre = groundLayer | LayerMask.GetMask("Plataformas");
        RaycastHit2D hit = Physics2D.Raycast(edgeCheckPared.position, transform.right * transform.localScale.x, checkDistancia, capasAgarre);

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