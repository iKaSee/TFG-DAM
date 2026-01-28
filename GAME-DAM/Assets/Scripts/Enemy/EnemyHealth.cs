using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public EnemyHealthBar healthBar;

    [Header("Visual Flash")]
    public SpriteRenderer spriteRenderer;
    public Color flashColor = Color.red; // Lo he puesto en rojo por defecto como querías
    public float flashDuration = 0.1f;
    private Color originalColor;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isBeingKnockbacked;

    [Header("Ajustes de Muerte")]
    public float tiempoParaDestruir = 2f; // Tiempo que dura la animación de muerte
    private bool estaMuerto = false;
    public bool esElBoss = false; // <--- OPCIÓN B: MARCAR SI ES EL JEFE FINAL


    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Guardamos el color original antes de que pase nada
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damage, Vector2 playerPosition)
    {
        if (estaMuerto) return; // Si ya murió, no procesamos más daño

        currentHealth -= damage;

        // 1. DETENEMOS TODO LO ANTERIOR PRIMERO
        StopAllCoroutines();

        // 2. ACTIVAMOS LOS EFECTOS
        StartCoroutine(FlashEffect());

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // 3. LÓGICA DE UI Y DESPERTAR
        if (healthBar != null && !healthBar.gameObject.activeSelf) healthBar.gameObject.SetActive(true);
        if (healthBar != null) healthBar.ConfigurarBarra((int)currentHealth, (int)maxHealth);

        if (GetComponent<BatAI>() != null) GetComponent<BatAI>().WakeUp();

        // --- NUEVO: También despertar al Boss si tiene BossAI ---
        if (GetComponent<BossAI>() != null) GetComponent<BossAI>().WakeUp();

        // 4. ANIMACIÓN Y MOVIMIENTO (FÍSICA)
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (anim != null) anim.SetTrigger("Hurt");
            StartCoroutine(ApplyKnockback(playerPosition));
        }
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (anim != null) anim.SetBool("IsDead", true);

        // --- SOLUCIÓN AL HUNDIMIENTO ---
        // Convertimos el Rigidbody en Static para que no le afecte la gravedad ni fuerzas
        rb.bodyType = RigidbodyType2D.Static;

        // Desactivamos el collider para que el jugador pueda pasar a través
        GetComponent<Collider2D>().enabled = false;

        // --- LÓGICA DE FINAL DE JUEGO (OPCIÓN B) ---
        if (esElBoss)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.TerminarJuego(); // Llama a la corrutina de los 10 segundos y cambio de escena
            }
        }

        // IMPORTANTE: No hacemos "this.enabled = false" porque detendría la corrutina de destrucción
        StartCoroutine(MuerteFinal());
    }

    // Corrutina para esperar a la animación y limpiar el objeto
    private IEnumerator MuerteFinal()
    {
        yield return new WaitForSeconds(tiempoParaDestruir);
        Destroy(gameObject);
    }

    public void DestroyEnemy() { Destroy(gameObject); }

    private IEnumerator ApplyKnockback(Vector2 playerPosition)
    {
        isBeingKnockbacked = true;
        Vector2 direction = ((Vector2)transform.position - playerPosition).normalized;
        rb.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        isBeingKnockbacked = false;
    }

    public bool IsInKnockback() => isBeingKnockbacked;
}