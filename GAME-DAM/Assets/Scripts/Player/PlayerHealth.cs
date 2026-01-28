using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;

    public HealthBarController healthBar;
    public DamageFlash damageFlash;

    private int currentHealth;


    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetVidaMaxima(maxHealth); // Aquí le enviamos el 100
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }
        healthBar.ActualizarVida(currentHealth);
        Debug.Log("Vida Player: " + currentHealth);

        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
        {
            healthBar.ActualizarVida(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hurt");
        }
    }

    // --- NUEVA FUNCIÓN PARA CURAR ---
    public void Heal(int cantidad)
    {
        if (isDead) return;

        currentHealth += cantidad;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Actualizamos la barra visual
        if (healthBar != null)
        {
            healthBar.ActualizarVida(currentHealth);
        }

        // Efecto visual: si quieres que brille verde, podrías pasarle un color al Flash
        // Por ahora usamos el mismo flash para indicar que algo cambió en la vida
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        Debug.Log("Jotem curado. Vida actual: " + currentHealth);
    }
    // --------------------------------

    void Die()
    {
        isDead = true;

        // 1. Ponemos los parámetros de movimiento a 0 para que no intenten volver al Idle/Run
        anim.SetFloat("HorizontalSpeed", 0);
        anim.SetFloat("VerticalVelocity", 0);
        anim.SetBool("isGrounded", true);

        // 2. Activamos la muerte
        anim.SetBool("isDead", true);

        // 3. Desactivamos los scripts
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;

        // Desactivamos el Rigidbody para que no se caiga por el suelo o ruede raro
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        FindObjectOfType<GameManager>().ShowGameOver();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}