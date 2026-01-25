using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{
    [Header("Deteccion")]
    public float detectionRange = 6f;
    public LayerMask playerLayer;
    private Transform playerTransform;
    private bool isPlayerDetected = false;

    [Header("Movimiento")]
    public float speed = 2.5f;
    public Transform groundCheck; // Un punto al borde del enemigo para no caerse
    private bool movingRight = true;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public int damage = 15;
    public float attackCooldown = 1.5f;
    private float nextAttackTime;

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
        // Si está en knockback no hace nada (Reutilizamos lógica del murciélago)
        if (health != null && health.IsInKnockback()) return;

        CheckForPlayer();

        if (isPlayerDetected)
        {
            HandleChase();
        }
        else
        {
            HandlePatrol();
        }
    }

    void CheckForPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null)
        {
            playerTransform = player.transform;
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    void HandlePatrol()
    {
        // Lógica de caminar de un lado a otro
        rb.linearVelocity = new Vector2(movingRight ? speed : -speed, rb.linearVelocity.y);

        // Girar si llega a un precipicio o pared (opcional)
        // Aquí podrías usar un Raycast hacia abajo en el groundCheck
    }

    void HandleChase()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            // Perseguimos a Jotem
            float direction = playerTransform.position.x > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            Flip(direction);
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        // Aquí llamarías al daño mediante Animation Event como en el murciélago
    }

    void Flip(float dir)
    {
        if (dir > 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }
}