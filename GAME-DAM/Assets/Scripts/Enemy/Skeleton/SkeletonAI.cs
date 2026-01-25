using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Deteccion")]
    public float detectionRange = 6f;
    public LayerMask playerLayer;
    private Transform playerTransform;
    private bool isPlayerDetected = false;
    private Transform targetPoint; // Buscaremos el EnemyTarget de Jotem
    public string targetPointName = "EnemyTarget";

    [Header("Movimiento")]
    public float speed = 2f;
    private bool facingRight = false;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float nextAttackTime;
    public int damage = 20;
    public Transform attackPoint;   // Objeto vacío "SkeletonAttackPoint"
    public float hitRadius = 0.6f;

    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth health;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        // Si el enemigo está recibiendo un golpe (Knockback), no hace nada
        if (health != null && health.IsInKnockback()) return;

        CheckForPlayer();

        if (isPlayerDetected)
        {
            HandleBehavior();
        }
        else
        {
            // Si no hay player, se queda quieto o patrulla
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("HorizontalSpeed", 0f);
        }
    }

    void CheckForPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (player != null)
        {
            playerTransform = player.transform;
            // Buscamos el punto de mira que creamos para Jotem
            if (targetPoint == null) targetPoint = playerTransform.Find(targetPointName);
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    void HandleBehavior()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Si está en rango de ataque
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Se detiene para atacar
            anim.SetFloat("HorizontalSpeed", 0f);

            if (Time.time >= nextAttackTime)
            {
                StartAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else // Si está lejos, lo persigue caminando
        {
            FollowPlayer();
        }
    }

    void StartAttack()
    {
        anim.SetTrigger("Attack");
    }

    // ESTA FUNCIÓN SE LLAMA DESDE UN ANIMATION EVENT (Igual que el murciélago)
    public void HitPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, hitRadius, playerLayer);

        if (hit != null)
        {
            PlayerHealth pHealth = hit.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage(damage);
            }
        }
    }

    void FollowPlayer()
    {
        // Calculamos dirección solo en el eje X (es terrestre)
        float directionX = playerTransform.position.x > transform.position.x ? 1 : -1;

        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
        anim.SetFloat("HorizontalSpeed", 1f);

        // Girar el sprite
        if (directionX > 0 && !facingRight) Flip();
        else if (directionX < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Dibujar los rangos en el Editor (Reutilizado del murciélago)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, hitRadius);
        }
    }
}