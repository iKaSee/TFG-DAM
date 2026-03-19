using UnityEngine;
using System.Collections.Generic; // NUEVO: Necesario para las listas
using Object = UnityEngine.Object;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    
    [Header("Configuracion de Ataque")]
    public float comboResetTime = 1.5f; 
    private float lastClickTime;
    
    [Header("Sistema de Clics")]
    public int clicks = 0; 
    public bool isAttacking;

    [Header("Deteccion de Daño")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    // --- NUEVO SISTEMA DE DAÑO CONTINUO ---
    public bool dañoActivado = false;
    private List<Collider2D> enemigosGolpeados = new List<Collider2D>();
    // --------------------------------------

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

   void Update()
    {
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null && pc.soloCaminar) return;

        // Mientras el daño esté activado, comprobamos impactos
        if (dañoActivado)
        {
            DeteccionContinua();
        }

        AnimatorStateInfo estadoActual = anim.GetCurrentAnimatorStateInfo(0);

        // --- NUEVO SISTEMA ANTI-COOLDOWN ---
        // Comprobamos si la animación que se está reproduciendo AHORA es un ataque
        bool estaHaciendoAtaque = estadoActual.IsName("Attack_1") || 
                                  estadoActual.IsName("Attack_2") || 
                                  estadoActual.IsName("Attack_3");

        // Si ya no está atacando (y ha pasado una décima de segundo desde que pulsaste F para que a Unity le dé tiempo a procesarlo)
        if (!estaHaciendoAtaque && (Time.time - lastClickTime > 0.1f))
        {
            clicks = 0;
            isAttacking = false;
            dañoActivado = false; // Apagamos el daño por seguridad
        }
        // -----------------------------------

        // El salvavidas de tiempo original por si las moscas
        if (Time.time - lastClickTime > comboResetTime)
        {
            clicks = 0;
            isAttacking = false;
            dañoActivado = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (clicks < 3)
            {
                lastClickTime = Time.time;
                clicks++;

                if (clicks == 1)
                {
                    isAttacking = true;
                    // El -1, 0f fuerza a la animación a empezar desde el fotograma 0 INSTANTÁNEAMENTE, sin retrasos
                    anim.Play("Attack_1", -1, 0f); 
                }
            }
        }
    }

    // --- EVENTOS DE ANIMACIÓN PARA EL COMBO ---
    public void CheckCombo2() { if (clicks >= 2) anim.Play("Attack_2"); }
    public void CheckCombo3() { if (clicks >= 3) anim.Play("Attack_3"); }

    // --- NUEVOS EVENTOS PARA EL DAÑO (Activar y Desactivar) ---
    public void ActivarDaño()
    {
        dañoActivado = true;
        enemigosGolpeados.Clear(); // Limpiamos la memoria para poder volver a pegar en el siguiente tajo
    }

    public void DesactivarDaño()
    {
        dañoActivado = false;
    }
    // ----------------------------------------------------------

    public void DeteccionContinua()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Si ya golpeamos a este enemigo en este mismo tajo, lo ignoramos y pasamos al siguiente
            if (enemigosGolpeados.Contains(enemy)) continue;

            // Si es un enemigo nuevo, lo añadimos a la lista y le hacemos daño
            enemigosGolpeados.Add(enemy);

            // 1. Lógica con el Boss (Referencia específica)
            BoosHealth bossHealth = enemy.GetComponent<BoosHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(attackDamage, transform.position);
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null) gm.SacudirCamara();
            }

            // 2. Lógica con Enemigos Genéricos (Como el Arquero)
            EnemyHealth genericHealth = enemy.GetComponent<EnemyHealth>();
            if (genericHealth != null)
            {
                genericHealth.TakeDamage(attackDamage);
            }

            // 3. Lógica con Cofres
            Chest chest = enemy.GetComponent<Chest>();
            if (chest != null)
            {
                chest.TakeDamage(1);
            }

            // 4. Lógica con Esqueletos antiguos 
            EnemySkeleton skeleton = enemy.GetComponent<EnemySkeleton>();
            if (skeleton != null)
            {
                skeleton.TakeDamage(1);
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null) gm.SacudirCamara();
            }

            // 5. Lógica con Elite Skeleton (MiniBoss)
            EliteSkeleton eliteSkeleton = enemy.GetComponent<EliteSkeleton>();
            if (eliteSkeleton != null)
            {
                eliteSkeleton.TakeDamage(attackDamage);
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null) gm.SacudirCamara(); 
            }
            
            // 6. Lógica con Asesino del Cofre (ChestAssassin)
            ChestAssassin chestAssassin = enemy.GetComponent<ChestAssassin>();
            if (chestAssassin != null)
            {
                chestAssassin.TakeDamage(attackDamage);
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null) gm.SacudirCamara(); 
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}