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
        currentHealth -= damage;

        // 1. DETENEMOS TODO LO ANTERIOR PRIMERO
        // Así el nuevo golpe resetea el flash y el knockback sin cancelarlos a mitad
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

        // 4. ANIMACIÓN Y MOVIMIENTO (FÍSICA)
        if (anim != null) anim.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback(playerPosition));

        if (currentHealth <= 0) Die();
    }

    private IEnumerator FlashEffect()
    {
        // Ponemos el color de impacto
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        // Volvemos al color que el bicho tenía en el Awake (normalmente blanco/gris)
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (anim != null) anim.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
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