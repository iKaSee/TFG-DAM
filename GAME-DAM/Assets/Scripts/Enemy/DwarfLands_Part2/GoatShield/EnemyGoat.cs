using UnityEngine;
using System.Collections;

public class EnemyGoat : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public float blockRange = 2.5f;

    [Header("Ajustes de Combate CON escudo")]
    public float attackRateShield = 2f;
    public int damageShield = 15;

    [Header("Ajustes de Combate SIN escudo")]
    public float attackRateNoShield = 1f;
    public int damageNoShield = 25;

    [Header("Ajustes Generales")]
    public float moveSpeedShield = 2f;
    public float moveSpeedNoShield = 3.5f;
    public int maxHealth = 500;

    [Header("Escudo")]
    public int shieldHits = 3;

    private int currentHealth;
    private int currentShieldHits;
    private bool hasShield = true;
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
        currentShieldHits = shieldHits;
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;
        if (estaAtacando) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Atacar();
        }
        else if (distanceToPlayer <= attackRange && Time.time < nextAttackTime && hasShield)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isBlocking", true);
        }
        else if (distanceToPlayer > attackRange && distanceToPlayer <= blockRange && hasShield)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isBlocking", true);
        }
        else if (distanceToPlayer > blockRange && distanceToPlayer <= detectionRange)
        {
            MoverHaciaJugador();
        }
        else if (distanceToPlayer > detectionRange)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isBlocking", false);
        }
    }

    void MoverHaciaJugador()
    {
        float speed = hasShield ? moveSpeedShield : moveSpeedNoShield;
        float direction = player.position.x - transform.position.x;

        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(player.position.x, transform.position.y),
            speed * Time.deltaTime);

        anim.SetBool("isWalking", true);
        anim.SetBool("isBlocking", false);

        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Atacar()
    {
        estaAtacando = true;
        anim.SetBool("isWalking", false);

        if (hasShield)
        {
            anim.SetTrigger("attack");
            anim.SetBool("isBlocking", true);
            nextAttackTime = Time.time + attackRateShield;
        }
        else
        {
            anim.SetTrigger("attack");
            nextAttackTime = Time.time + attackRateNoShield;
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
            if (health != null)
            {
                int damage = hasShield ? damageShield : damageNoShield;
                health.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (hasShield)
        {
            currentShieldHits -= 1;
            estaAtacando = false;
            anim.SetTrigger("hit");
            if (currentShieldHits <= 0) RomperEscudo();
        }
        else
        {
            currentHealth -= damage;
            estaAtacando = false;
            anim.SetTrigger("hit");
            if (currentHealth <= 0) Die();
        }
    }

    void RomperEscudo()
    {
        hasShield = false;
        anim.SetTrigger("shieldBroken");
    }

    // Se activa por Animation Event al final de Shield_Cracking
    public void FinEscudo()
    {
        anim.SetBool("isBlocking", false);
    }

    // Se activa por Animation Event al final de Attack_End
    public void FinAtaque()
    {
        estaAtacando = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
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
