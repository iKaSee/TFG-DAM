using UnityEngine;
using System.Collections;

public class Chaser_Goat : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Ajustes de Combate")]
    public float attackRate = 1.2f;
    public int damage = 25;
    [Range(0f, 1f)] public float dodgeAttackChance = 0.2f;
    [Range(0f, 1f)] public float jumpAttackChance = 0.2f;

    [Header("Ajustes Generales")]
    public float moveSpeed = 4f;
    public int maxHealth = 300;

    [SerializeField] private BarraVidaEnemigo barraVida;

    private int currentHealth;
    private bool isDead = false;
    private bool estaAtacando = false;
    private float nextAttackTime = 0f;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;

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

        if (random < dodgeAttackChance)
        {
            anim.SetTrigger("dodgeAttack");
        }
        else if (random < dodgeAttackChance + jumpAttackChance)
        {
            anim.SetTrigger("jumpAttack");
        }
        else
        {
            anim.SetTrigger("attack");
        }
    }

    // Se activa por Animation Event
    public void GolpeEnemigo()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("hit");

        if (barraVida != null) barraVida.ActualizarVida(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (barraVida != null) barraVida.OcultarBarra();
        anim.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;

        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(DesactivarTrasMuerte(2f));
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
