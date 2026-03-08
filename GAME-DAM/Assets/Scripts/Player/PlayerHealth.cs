using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;

    public HealthBarController healthBar;
    public DamageFlash damageFlash;

    private int currentHealth;

    [Header("Invulnerabilidad Post-Daño")]
    public float tiempoInvulnerable = 1f; // 1 segundo de calma
    private bool esInvulnerable = false;

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

        PlayerController controller = GetComponent<PlayerController>();
        if (isDead || esInvulnerable || (controller != null && controller.invulnerable))
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
           
            if (healthBar != null) healthBar.ActualizarVida(0);
            Die();
        }
        else
        {
            if (healthBar != null) healthBar.ActualizarVida(currentHealth);
            if (damageFlash != null) damageFlash.Flash();
            anim.SetTrigger("Hurt");
            StartCoroutine(ActivarInvulnerabilidadTemporal());
        }


        if (currentHealth <= 0)
        {
            Die(); // Si es 0, morimos YA.
        }
        else
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(ActivarInvulnerabilidadTemporal());
        }
    }


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

    private IEnumerator ActivarInvulnerabilidadTemporal()
    {
        esInvulnerable = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        float tiempoPasado = 0;

        while (tiempoPasado < tiempoInvulnerable)
        {
            // En lugar de desactivar el objeto, lo ponemos medio transparente
            c.a = (c.a == 1f) ? 0.2f : 1f;
            sr.color = c;

            yield return new WaitForSeconds(0.1f);
            tiempoPasado += 0.1f;
        }

        // Al terminar, siempre opaco y blanco
        c.a = 1f;
        sr.color = Color.white;
        sr.enabled = true;
        esInvulnerable = false;
    }



    public void AplicarVeneno(int damagePorTic, int cantidadDeTics, float intervalo)
    {
        // Detenemos el veneno anterior si ya estaba activo para que no se acumulen infinitos
        StopCoroutine(nameof(RutinaVeneno));
        StartCoroutine(RutinaVeneno(damagePorTic, cantidadDeTics, intervalo));
    }


    IEnumerator RutinaVeneno(int damage, int tics, float espera)
    {
        for (int i = 0; i < tics; i++)
        {
            if (isDead) yield break; // Si Jotem muere, paramos el veneno

            // 1. Restar vida (Usamos tu variable 'currentHealth')
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;

            // 2. Actualizar tu barra de vida real
            if (healthBar != null)
            {
                healthBar.ActualizarVida(currentHealth);
            }

            // 3. Feedback Visual (Jotem parpadea en verde)
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.1f, 0.5f, 0.1f);

            yield return new WaitForSeconds(0.15f);
            if (sr != null) sr.color = Color.white;

            // Comprobamos si ha muerto por el tic de veneno
            if (currentHealth <= 0)
            {
                Die();
                yield break;
            }

            yield return new WaitForSeconds(espera);
        }
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Bloqueo físico inmediato
        rb.linearVelocity = Vector2.zero;

        // Desactivamos controles para que no pueda moverse ni atacar mientras muere
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;

        // 2. Lanzamos la animación (el Trigger que configuramos)
        anim.SetTrigger("DieTrigger");
        anim.SetBool("isDead", true);

        // 3. Esperamos un poco a que caiga al suelo antes de pausar y mostrar el menú
        // Llamamos a una función después de 1.5 segundos (ajusta según dure tu animación)
        Invoke("FinalizarMuerte", 1.5f);
    }

    void FinalizarMuerte()
    {
        // Detenemos el tiempo del juego (esto para enemigos, proyectiles, etc.)
        Time.timeScale = 0f;

        // Mostramos la pantalla de Game Over
        if (FindObjectOfType<GameManager>() != null)
        {
            FindObjectOfType<GameManager>().ShowGameOver();
        }
        else
        {
            Debug.LogWarning("No se encontró el GameManager en la escena.");
        }

    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // <--- ¡OBLIGATORIO VOLVER A 1!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}