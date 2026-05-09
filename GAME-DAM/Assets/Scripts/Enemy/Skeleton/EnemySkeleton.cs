using UnityEngine;
using System.Collections; // Arregla el error CS0246 (IEnumerator)

public class EnemySkeleton : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 6f;
    public float chaseRange = 4f;
    public float stopRange = 1.2f;

    [Header("Ajustes de Combate")]
    public float attackRange = 1.5f;
    public float attackRate = 1.5f;
    public int damageAmount = 20; // Arregla el error CS0103 (damageAmount)
    private float nextAttackTime = 0f;

    [Header("Ajustes Generales")]
    public float moveSpeed = 2f;
    public int maxHealth = 3;
    private int currentHealth;

    [SerializeField] private BarraVidaEnemigo barraVida;

    private Animator anim;
    private Transform player;
    private bool hasSpawned = false;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!hasSpawned && distanceToPlayer <= detectionRange)
        {
            SpawnSkeleton();
        }

        if (hasSpawned && !isDead)
        {
            // L�gica de ATAQUE
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    Atacar();
                    nextAttackTime = Time.time + attackRate;
                }
            }
            // L�gica de PERSECUCI�N
            else if (distanceToPlayer <= chaseRange && distanceToPlayer > stopRange)
            {
                MoverHaciaJugador();
            }
            else
            {
                anim.SetBool("isMoving", false);
            }
        }
    }

    void SpawnSkeleton()
    {
        hasSpawned = true;
        anim.SetTrigger("Spawn");
    }

    void MoverHaciaJugador()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Skeleton-out")) return;

        float direction = player.position.x - transform.position.x;
        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(player.position.x, transform.position.y),
            moveSpeed * Time.deltaTime);

        anim.SetBool("isMoving", true);

        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Atacar()
    {
        anim.SetBool("isMoving", false);
        anim.SetTrigger("Attack");
    }

    // Se activa por Animation Event
    public void GolpeEnemigo()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        anim.SetTrigger("Hit");
        if (barraVida != null) barraVida.ActualizarVida(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (barraVida != null) barraVida.OcultarBarra();
        anim.SetBool("isDead", true);
        anim.SetBool("isMoving", false);
        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(DesactivarTrasMuerte(1.4f));
    }

    IEnumerator DesactivarTrasMuerte(float retraso)
    {
        yield return new WaitForSeconds(retraso);
        anim.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}