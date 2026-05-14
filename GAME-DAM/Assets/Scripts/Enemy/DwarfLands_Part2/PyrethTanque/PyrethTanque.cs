using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PyrethTanque : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 2.5f;

    [Header("Ajustes de Combate")]
    public float attackRate = 2f;
    public int damageNormal = 30;
    public int damageLaser = 50;
    [Range(0f, 1f)] public float normalAttackChance = 0.8f;

    [Header("Ajustes Generales")]
    public float moveSpeed = 2.5f;
    public int maxHealth = 1500;

    [Header("Laser")]
    public GameObject laserPrefab;
    public Transform puntoDisparo;

    [SerializeField] private BossHealthBar bossHealthBar;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

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

        if (random < normalAttackChance)
        {
            anim.SetTrigger("attack");
        }
        else
        {
            anim.SetTrigger("laserAttack");
            StartCoroutine(DispararLaser());
        }
    }

    IEnumerator DispararLaser()
    {
        yield return new WaitForSeconds(0.5f);
        if (isDead || laserPrefab == null || puntoDisparo == null) yield break;

        GameObject laser = Instantiate(laserPrefab, puntoDisparo.position, Quaternion.identity);

        if (player != null)
        {
            laser.GetComponent<LaserProyectil>().Setup((player.position - puntoDisparo.position).normalized);
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
            if (health != null) health.TakeDamage(damageNormal);
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
        StartCoroutine(FinDelJuego());
    }

    IEnumerator FinDelJuego()
    {
        yield return new WaitForSeconds(5f);

        if (fadeCanvasGroup != null)
        {
            float duracion = 2f;
            float t = 0f;
            fadeCanvasGroup.alpha = 0f;
            while (t < duracion)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duracion);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene("EscenaFinal");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
