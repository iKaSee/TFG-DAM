using UnityEngine;

public class BatAI : MonoBehaviour
{
    [Header("Deteccion")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    private bool awake = false;

    [Header("Movimiento")]
    public float speed = 3f;
    private Transform playerTransform;
    // --- NUEVO: Referencia al punto de mira específico ---
    private Transform targetPoint;
    public string targetPointName = "EnemyTarget";
    // -----------------------------------------------------

    [Header("Vuelo Erratico")]
    public float frecuenciaOscilacion = 5f; // Qué tan rápido hace el zig-zag
    public float amplitudOscilacion = 0.5f; // Qué tan ancho es el zig-zag

    [Header("Ataque")]
    public float attackRange = 1.2f;    // Distancia para empezar a atacar
    public float attackCooldown = 2f;   // Tiempo entre ataques
    private float nextAttackTime;
    public int damage = 10;             // Daño que hace al Player
    public Transform attackPoint;       // Objeto vacío "BatAttackPoint"
    public float hitRadius = 0.5f;      // Radio del mordisco/golpe

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth health; // Referencia para el Knockback

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        // Si el enemigo está recibiendo un golpe (Knockback), no hace nada
        if (health != null && health.IsInKnockback()) return;

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

            // --- NUEVO: Buscar el punto de mira dentro del Player ---
            targetPoint = playerTransform.Find(targetPointName);
            // -------------------------------------------------------

            WakeUp();
        }
    }

    public void WakeUp() // Ahora es PUBLIC para que EnemyHealth pueda despertarlo al recibir daño
    {
        if (awake) return;
        awake = true;

        // --- AÑADIDO: Activar la barra de vida al despertar ---
        if (health != null && health.healthBar != null)
        {
            health.healthBar.gameObject.SetActive(true);
        }
        // -------------------------------------------------------

        anim.SetBool("isAwake", true);
    }

    void HandleBehavior()
    {
        if (playerTransform == null) return;

        // --- MODIFICADO: La distancia se calcula ahora respecto al targetPoint si existe ---
        Vector3 currentTargetPos = targetPoint != null ? targetPoint.position : playerTransform.position;
        float distanceToTarget = Vector2.Distance(transform.position, currentTargetPos);

        // Si está en rango de ataque
        if (distanceToTarget <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; // Se detiene para atacar

            if (Time.time >= nextAttackTime)
            {
                StartAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else // Si está lejos, lo persigue
        {
            FollowPlayer();
        }
    }

    void StartAttack()
    {
        anim.SetTrigger("Attack");
    }

    // ESTA FUNCIÓN SE LLAMA DESDE UN ANIMATION EVENT
    public void HitPlayer()
    {
        // Detectamos si el Player sigue ahí en el momento del impacto visual
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
        // --- MODIFICADO: Ahora se dirige al targetPoint (pecho/cabeza) y no a los pies ---
        Vector3 currentTargetPos = targetPoint != null ? targetPoint.position : playerTransform.position;
        Vector2 direction = (currentTargetPos - transform.position).normalized;

        // --- NUEVO: MOVIMIENTO ERRÁTICO (Zig-Zag) ---
        // Creamos un vector perpendicular a la dirección del movimiento
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        // Usamos Sin(Time) para crear el vaivén
        float oscilacion = Mathf.Sin(Time.time * frecuenciaOscilacion) * amplitudOscilacion;

        // Sumamos la oscilación a la dirección principal
        Vector2 direccionFinal = direction + (perpendicular * oscilacion);
        // --------------------------------------------

        rb.linearVelocity = direccionFinal.normalized * speed;

        // Girar el sprite hacia el jugador
        if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }

    // Dibujar los rangos en el Editor
    void OnDrawGizmosSelected()
    {
        // Rango de visión (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rango de ataque (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Radio del impacto real (Cian)
        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, hitRadius);
        }
    }
}