using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Deteccion")]
    public float detectionRange = 12f;
    public LayerMask playerLayer;
    private bool awake = false;

    [Header("Movimiento Boss")]
    public float speed = 3.5f;
    private Transform playerTransform;
    private Transform targetPoint;
    public string targetPointName = "EnemyTarget";

    [Header("Ataque Boss")]
    public float attackRange = 2.5f;    // Distancia para frenar y atacar
    public float attackCooldown = 2.5f; // Tiempo entre ataques
    private float nextAttackTime;
    public int damage = 30;             // Daño del Jefe
    public Transform attackPoint;       // Objeto hijo "BossAttackPoint"
    public float hitRadius = 1.2f;      // Radio del golpe del Jefe

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth health; // Reutilizamos tu EnemyHealth

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        // Si el jefe está muerto (en EnemyHealth), no hacemos nada
        // Asumo que tu EnemyHealth tiene un bool o lógica de muerte

        // Si está en Knockback (por el Hit), no se mueve
        if (health != null && health.IsInKnockback())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!awake)
        {
            CheckForPlayer();
        }
        else
        {
            HandleBehavior();
        }
    }

    void CheckForPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerCollider != null)
        {
            playerTransform = playerCollider.transform;
            targetPoint = playerTransform.Find(targetPointName);
            WakeUp();
        }
    }

    public void WakeUp()
    {
        if (awake) return;
        awake = true;

        // Mostrar barra de vida del Boss si existe
        if (health != null && health.healthBar != null)
        {
            health.healthBar.gameObject.SetActive(true);
        }

        // Si tienes una animación de "despertar" o grito, úsala aquí. 
        // Si no, simplemente activamos el movimiento.
    }

    void HandleBehavior()
    {
        if (playerTransform == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToTarget <= attackRange)
        {
            // FRENAR Y ATACAR
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Velocidad", 0); // Tu animación de Idle

            if (Time.time >= nextAttackTime)
            {
                StartAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            FollowPlayer();
        }
    }

    void StartAttack()
    {
        anim.SetTrigger("Attack"); // Trigger de tu animación 'atack'
    }

    // FUNCIÓN PARA ANIMATION EVENT (Frame de impacto)
    public void HitPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, hitRadius, playerLayer);
        if (hit != null)
        {
            PlayerHealth pHealth = hit.GetComponent<PlayerHealth>();
            if (pHealth != null) pHealth.TakeDamage(damage);
        }
    }

    void FollowPlayer()
    {
        // Solo nos movemos en el eje X para que sea un Boss terrestre
        float direccionX = (playerTransform.position.x > transform.position.x) ? 1 : -1;

        rb.linearVelocity = new Vector2(direccionX * speed, rb.linearVelocity.y);

        anim.SetFloat("Velocidad", 1); // Activa la animación 'run' (si usas un float Velocidad)
        // Si usas un Bool para correr, cambia a: anim.SetBool("isRunning", true);

        // Girar el sprite
        if (direccionX > 0) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

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