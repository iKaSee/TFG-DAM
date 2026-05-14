using UnityEngine;
using System.Collections;

public class PyrethAsesino : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 12f;
    public float attackRange = 2f;

    [Header("Ajustes de Combate")]
    public float attackRate = 1.5f;
    public int damageBasic = 20;
    public int damageHeavy = 40;
    public int damageDive = 35;
    [Range(0f, 1f)] public float basicAttackChance = 0.5f;
    [Range(0f, 1f)] public float heavyAttackChance = 0.3f;
    [Range(0f, 1f)] public float diveAttackChance = 0.2f;

    [Header("Ajustes Generales")]
    public float moveSpeed = 5f;
    public int maxHealth = 800;

    [Header("Dive Attack")]
    public float diveForce = 15f;
    public float diveForwardForce = 8f;

    [SerializeField] private BossHealthBar bossHealthBar;

    private int currentHealth;
    private bool isDead = false;
    private bool estaAtacando = false;
    private float nextAttackTime = 0f;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;

    private int currentAttackDamage;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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

        if (estaAtacando)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Idle"))
            {
                estaAtacando = false;
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Atacar();
        }
        else if (distanceToPlayer > attackRange && distanceToPlayer <= detectionRange)
        {
            MoverHaciaJugador();
        }
        else if (distanceToPlayer > detectionRange)
        {
            anim.SetBool("isRunning", false);
        }
    }

    void MoverHaciaJugador()
    {
        if (bossHealthBar != null && !bossHealthBar.EstaActiva()) bossHealthBar.IniciarBarra(maxHealth);

        float direction = player.position.x - transform.position.x;

        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(player.position.x, transform.position.y),
            moveSpeed * Time.deltaTime);

        anim.SetBool("isRunning", true);

        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Atacar()
    {
        estaAtacando = true;
        anim.SetBool("isRunning", false);
        nextAttackTime = Time.time + attackRate;

        float random = Random.value;

        if (random < basicAttackChance)
        {
            currentAttackDamage = damageBasic;
            anim.SetTrigger("attack");
        }
        else if (random < basicAttackChance + heavyAttackChance)
        {
            currentAttackDamage = damageHeavy;
            anim.SetTrigger("heavyAttack");
            StartCoroutine(EjecutarHeavyAttack());
        }
        else
        {
            currentAttackDamage = damageDive;
            anim.SetTrigger("diveAttack");
            StartCoroutine(EjecutarDiveAttack());
        }
    }

    IEnumerator EjecutarHeavyAttack()
    {
        yield return new WaitForSeconds(0.3f);
        if (isDead || rb == null) yield break;

        rb.AddForce(Vector2.up * diveForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        if (isDead || rb == null || player == null) yield break;

        Vector2 direccionJugador = (player.position - transform.position).normalized;
        rb.AddForce(direccionJugador * diveForce, ForceMode2D.Impulse);
    }

    IEnumerator EjecutarDiveAttack()
    {
        yield return new WaitForSeconds(0.3f);
        if (isDead || rb == null || player == null) yield break;

        Vector2 direccionJugador = (player.position - transform.position).normalized;
        Vector2 fuerza = Vector2.up * diveForce + direccionJugador * diveForwardForce;
        rb.AddForce(fuerza, ForceMode2D.Impulse);
    }

    // Se activa por Animation Event
    public void GolpeEnemigo()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(currentAttackDamage);
        }
    }

    // Se activa por Animation Event al final del ataque
    public void FinAtaque()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Idle"))
        {
            estaAtacando = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("hit");

        if (bossHealthBar != null) bossHealthBar.ActualizarBarra(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (bossHealthBar != null) bossHealthBar.OcultarBarra();
        anim.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;

        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(DesactivarTrasMuerte(2f));
    }

    IEnumerator DesactivarTrasMuerte(float retraso)
    {
        yield return new WaitForSeconds(retraso);
        anim.enabled = false;
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
