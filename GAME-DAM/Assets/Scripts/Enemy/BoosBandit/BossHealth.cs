using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class BoosHealth : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool estaMuerto = false;

    [Header("Ajustes de Aturdimiento (Cooldown)")]
    public float tiempoEntreDolor = 1.5f; 
    private float tiempoSiguienteDolor = 0f;

    [Header("Referencias")]
    public BossHealthBarController uiBoss;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    [Header("Fase 2")]
    private bool fase2Activada = false;

    [Header("Fase 3")]
    private bool fase3Activada = false;   

    [Header("Referencias de Fin de Combate")]
    public GameObject[] murosAdesactivar;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Vector2 playerPos)
    {
        if (estaMuerto) return;

        currentHealth -= damage;
        if (uiBoss != null) uiBoss.ActualizarBarra(currentHealth, maxHealth);

        StartCoroutine(FlashEfecto());

        BossGuardian scriptIA = GetComponent<BossGuardian>();
        if (scriptIA != null)
        {
            if (!fase2Activada && currentHealth <= maxHealth * 0.6f && currentHealth > maxHealth * 0.3f)
            {
                fase2Activada = true;
                scriptIA.EntrarEnFaseBerserker();
            }

            if (!fase3Activada && currentHealth <= maxHealth * 0.3f)
            {
                fase3Activada = true;
                scriptIA.EntrarEnFase3();
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Solo se quejará cada 1.5 segundos (tu lógica original)
            if (Time.time >= tiempoSiguienteDolor)
            {
                if (anim != null) anim.SetTrigger("Hurt");
                tiempoSiguienteDolor = Time.time + tiempoEntreDolor;
            }
        }
    }    IEnumerator FlashEfecto()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        // Si está en fase 2, vuelve a su rojo de furia, si no, a blanco
        if (fase2Activada) sprite.color = new Color(1f, 0.6f, 0.6f);
        else sprite.color = Color.white;
    }

    void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        // Limpieza de IA y partículas
        BossGuardian scriptIA = GetComponent<BossGuardian>();
        if (scriptIA != null)
        {
            scriptIA.ApagarFaseBerserker();
            scriptIA.enabled = false;
        }

        // UI y Muros
        if (uiBoss != null) uiBoss.DesactivarBarra();

        if (murosAdesactivar != null)
        {
            foreach (GameObject muro in murosAdesactivar)
            {
                if (muro != null) muro.SetActive(false);
            }
        }

        // Animación y Física
        if (anim != null) anim.SetBool("IsDead", true);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;

        Debug.Log("Boss derrotado.");
        IntroCutscene cinematica = Object.FindFirstObjectByType<IntroCutscene>();
        if (cinematica != null && cinematica.musicaBoss != null)
        {
            cinematica.musicaBoss.Stop();
        }
        // ------------------------------------------------

        BossMusicController music = Object.FindAnyObjectByType<BossMusicController>();
        if (music != null) music.PararTodo();

        StartCoroutine(EsperarYCargarFinal());
        // ----------------------------------------
    }

    IEnumerator EsperarYCargarFinal()
    {
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene("EscenaFinal"); 
    }
}