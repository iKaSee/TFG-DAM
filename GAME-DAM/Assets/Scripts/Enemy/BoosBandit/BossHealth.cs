using UnityEngine;
using System.Collections;

public class BoosHealth : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool estaMuerto = false;

    [Header("Ajustes de Aturdimiento (Cooldown)")]
    public float tiempoEntreDolor = 1.5f; // Solo se quejará cada 1.5 segundos
    private float tiempoSiguienteDolor = 0f;

    [Header("Referencias")]
    public BossHealthBarController uiBoss;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    [Header("Fase 2")]
    private bool fase2Activada = false;

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

        if (!fase2Activada && currentHealth <= maxHealth / 2)
        {
            fase2Activada = true;
            BossGuardian guardian = GetComponent<BossGuardian>();
            if (guardian != null) guardian.EntrarEnFaseBerserker();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // --- PASO 2: SUPER ARMADURA ---
            // Comprobamos si el Boss está en la animación de "Attack"
            // Si el animador está en el estado de ataque, NO hacemos el Hurt
            bool estaAtacando = anim.GetCurrentAnimatorStateInfo(0).IsName("Atack_Bandit");

            if (Time.time >= tiempoSiguienteDolor && !estaAtacando)
            {
                if (anim != null) anim.SetTrigger("Hurt");
                tiempoSiguienteDolor = Time.time + tiempoEntreDolor;
            }
        }
    }
    IEnumerator FlashEfecto()
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
        BossMusicController music = Object.FindAnyObjectByType<BossMusicController>();
        if (music != null) music.PararTodo(); ;
    }
}   