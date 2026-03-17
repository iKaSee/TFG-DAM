using UnityEngine;

public class EliteSkeleton : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Estadísticas de Movimiento")]
    public float walkSpeed = 3f;
    
    [Header("Ajustes de Slide Attack")]
    public float slideSpeed = 12f;
    public float maxSlideDistance = 10f; 
    public float maxSlideTime = 1.5f;    
    private bool isSliding = false;
    private Vector2 slideTargetPosition; 
    private float slideTimer = 0f; 


    [Header("Ajustes de Combate (Normal)")]
    public int damageNormal = 20;
    public float attackRange = 1.5f;  
    public float attackCooldown = 1.5f;
    private float nextAttackTime;
    private bool isAttacking = false; 


    [Header("Ajustes de Combate (Slide)")]
    public int damageSlide = 30;
    public float chaseRange = 10f;    
    public float slideRange = 6f;     
    public float slideCooldown = 4f;
    private float nextSlideTime;


    [Header("Detección de Daño")]
    public Transform attackPoint;    
    public float attackRadius = 1.2f;
    public LayerMask playerLayer;    

    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (isAttacking)
        {
            LookAtPlayer();
            return; 
        }

        if (isSliding)
        {
            ControlarDistanciaSlide();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            DecidirAccion(distanceToPlayer);
        }
        else
        {
            Parar();
        }
    }

    void DecidirAccion(float distance)
    {
        LookAtPlayer();

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AtacarNormal();
        }
        else if (distance <= slideRange && distance > attackRange + 1f && Time.time >= nextSlideTime)
        {
            LanzarSlide();
        }
        else if (distance > attackRange)
        {
            Perseguir();
        }
    }

    void Perseguir()
    {
        anim.SetBool("isMoving", true);
        float dir = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);
    }

    void Parar()
    {
        anim.SetBool("isMoving", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    void AtacarNormal()
    {
        Parar();
        isAttacking = true; 
        anim.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    void LanzarSlide()
    {
       Parar();
        isAttacking = true; 
        anim.SetTrigger("SlideAttack");
        nextSlideTime = Time.time + slideCooldown;
       
    }

    void ControlarDistanciaSlide()
    {
        slideTimer += Time.deltaTime; 

        float currentX = transform.position.x;
        float targetX = slideTargetPosition.x;
        float dir = transform.localScale.x > 0 ? 1 : -1;

        rb.linearVelocity = new Vector2(dir * slideSpeed, rb.linearVelocity.y);

        bool llegoALaMeta = (dir == 1 && currentX >= targetX) || (dir == -1 && currentX <= targetX);
        
        if (llegoALaMeta || slideTimer >= maxSlideTime)
        {
            FinalizarDeslizamiento();
            anim.Play("Slide_End"); 
        }
    }

    // --- EVENTOS DE ANIMACIÓN ---

    public void FinalizarAtaqueNormal()
    {
        isAttacking = false;
    }

    public void GolpeCuerpoACuerpo()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hitPlayer != null) hitPlayer.GetComponent<PlayerHealth>().TakeDamage(damageNormal);
    }

    public void ActivarDeslizamiento() 
    {
        isAttacking = false; 
        isSliding = true;    
        slideTimer = 0f;
        float dir = transform.localScale.x > 0 ? 1 : -1;
        slideTargetPosition = new Vector2(transform.position.x + (maxSlideDistance * dir), transform.position.y);}

    public void FinalizarDeslizamiento() 
    {
        isSliding = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public void DeteccionDañoDeslizante()
    {
        Vector2 centroDelPecho = new Vector2(transform.position.x, transform.position.y + 1f);
        
        Collider2D hitPlayer = Physics2D.OverlapCircle(centroDelPecho, attackRadius, playerLayer);
        
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerHealth>().TakeDamage(damageSlide);
        }
    }

    public void Die()
    {
        isDead = true;
        isAttacking = false;
        isSliding = false;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slideRange); 
    }
}