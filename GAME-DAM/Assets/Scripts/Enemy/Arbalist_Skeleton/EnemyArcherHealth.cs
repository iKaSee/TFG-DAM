using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    public int maxHealth = 60;
    private int currentHealth;

    [Header("Efectos")]
    public GameObject bloodEffect;

    [SerializeField] private BarraVidaEnemigo barraVida;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (bloodEffect != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
            GameObject effect = Instantiate(bloodEffect, spawnPos, Quaternion.identity);
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                Vector3 direction = (transform.position - player.transform.position).normalized;
                effect.transform.right = direction; 
            }

            Destroy(effect, 1.5f); // Limpieza de memoria
        }

        if (anim != null) anim.SetTrigger("Hurt");

        if (barraVida != null) barraVida.ActualizarVida(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (barraVida != null) barraVida.OcultarBarra();

        EnemyArcher archerScript = GetComponent<EnemyArcher>();
        if (archerScript != null) archerScript.Die();

        if (anim != null) 
        {
            anim.SetTrigger("Die");
            anim.Play("Death"); 
        }

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2.5f);
    }
}