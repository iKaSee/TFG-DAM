using UnityEngine;
using System.Collections; // Necesario para la corrutina

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 100f;
    private float currentHealth;

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
    }

    public void TakeDamage(float damage, Vector2 playerPosition)
    {
        currentHealth -= damage;

        // Despertar al murciélago si estaba dormido
        if (GetComponent<BatAI>() != null) GetComponent<BatAI>().WakeUp();

        if (anim != null) anim.SetTrigger("Hurt");

        // Aplicar retroceso
        StopAllCoroutines();
        StartCoroutine(ApplyKnockback(playerPosition));

        if (currentHealth <= 0) Die();
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
        if (anim != null) anim.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero; // Que no salga volando al morir
        this.enabled = false;
    }

    // Propiedad para que la IA sepa si debe quedarse quieta mientras recibe el golpe
    public bool IsInKnockback() => isBeingKnockbacked;
}