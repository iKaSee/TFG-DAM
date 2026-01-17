using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para reiniciar el nivel

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

        if (GetComponent<BatAI>() != null) GetComponent<BatAI>().WakeUp();

        if (anim != null) anim.SetTrigger("Hurt");

        StopAllCoroutines();
        StartCoroutine(ApplyKnockback(playerPosition));

        if (currentHealth <= 0) Die();
    }

    public void DestroyEnemy()
    {
       // Destroy(gameObject);
    }

    private IEnumerator ApplyKnockback(Vector2 playerPosition)
    {
        isBeingKnockbacked = true;
        Vector2 direction = ((Vector2)transform.position - playerPosition).normalized;
        rb.linearVelocity = direction * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isBeingKnockbacked = false;
    }

    void Die()
    {
        // 1. Evitamos que la muerte se ejecute más de una vez
        if (GetComponent<Collider2D>().enabled == false) return;

        // 2. Visual y física
        if (anim != null) anim.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 3. Desactivar la IA para que no de errores mientras espera
        if (GetComponent<BatAI>() != null) GetComponent<BatAI>().enabled = false;

        // 4. Reinicia la escena actual tras 5 segundos (como en tu Player)
        Invoke("RestartLevel", 2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsInKnockback() => isBeingKnockbacked;
}