using UnityEngine;

public class ChestAssassin : MonoBehaviour
{
    [Header("Estadísticas")]
    public int maxHealth = 60;
    private int currentHealth;

    [Header("Movimiento")]
    public float speed = 4f;
    public float chaseRange = 10f; 

    [Header("Combate")]
    public int attackDamage = 20;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;

    [Header("Detección y Daño")]
    public Transform attackPoint; 
    public float attackRadius = 0.8f;
    public LayerMask playerLayer; 

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (isHit || isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Atacar();
            }
            else
            {
                Parar();
                MirarAlJugador();
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            Perseguir();
        }
        else
        {
            Parar();
        }
    }

    void Perseguir()
    {
        anim.SetBool("isMoving", true);
        MirarAlJugador();

        float dir = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void Parar()
    {
        anim.SetBool("isMoving", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void Atacar()
    {
        Parar();
        isAttacking = true;
        anim.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    void MirarAlJugador()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Parar();
            isAttacking = false;
            isHit = true;
            anim.SetTrigger("Hit");
        }
    }

    public void Die()
    {
        isDead = true;
        isAttacking = false;
        isHit = false;
        
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        
        anim.SetTrigger("Die");
        
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    public void FinalizarHit()
    {
        isHit = false;
    }

    public void FinalizarAtaque()
    {
        isAttacking = false;
    }

    public void GolpeCuerpoACuerpo()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}