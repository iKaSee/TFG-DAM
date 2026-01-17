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
            healthBar.SetVidaMaxima(maxHealth);
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

    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        GetComponent<PlayerController>().enabled = false;
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