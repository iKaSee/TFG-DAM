using UnityEngine;
using System.Collections; // Necesario para la corrutina

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public EnemyHealthBar healthBar; // <--- AÑADIDO: Referencia a la barra flotante

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

        // Quitamos la activación del Awake para que la barra no aparezca 
        // hasta que el BatAI detecte al jugador o reciba daño.
    }

    public void TakeDamage(float damage, Vector2 playerPosition)
    {
        currentHealth -= damage;

        // Aseguramos que la barra se active al recibir daño
        if (healthBar != null && !healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(true);
        }

        // Actualizar la barra visualmente
        if (healthBar != null)
        {
            healthBar.ConfigurarBarra((int)currentHealth, (int)maxHealth);
        }

        // Despertar al murciélago si estaba dormido
        if (GetComponent<BatAI>() != null) GetComponent<BatAI>().WakeUp();

        if (anim != null) anim.SetTrigger("Hurt");

        // Aplicar retroceso
        StopAllCoroutines();
        StartCoroutine(ApplyKnockback(playerPosition));

        if (currentHealth <= 0) Die();
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private IEnumerator ApplyKnockback(Vector2 playerPosition)
    {
        isBeingKnockbacked = true;

        // Calculamos la dirección opuesta al jugador
        Vector2 direction = ((Vector2)transform.position - playerPosition).normalized;

        // Aplicamos la fuerza de golpe
        rb.linearVelocity = direction * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        // Volvemos a dejar la velocidad a cero para que la IA recupere el control
        rb.linearVelocity = Vector2.zero;
        isBeingKnockbacked = false;
    }

    void Die()
    {
        // Ocultar la barra al morir para que no flote sobre el cadáver
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        if (anim != null) anim.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero; // Que no salga volando al morir
        this.enabled = false;
    }

    // Propiedad para que la IA sepa si debe quedarse quieta mientras recibe el golpe
    public bool IsInKnockback() => isBeingKnockbacked;
}