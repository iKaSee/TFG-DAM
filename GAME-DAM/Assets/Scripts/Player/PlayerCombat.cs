using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;

    [Header("Configuracion de Ataque")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public bool isAttacking;

    [Header("Deteccion de Daño")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swingSound;

    [Header("Ajustes de Combo")]
    public int comboStep = 0;
    public float comboResetTime = 0.5f;
    private float lastClickTime;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Lógica para reiniciar el combo
        if (Time.time - lastClickTime > comboResetTime)
        {
            ResetCombo();
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    // 1. Esta función SOLO lanza la animación
    void Attack()
    {
        isAttacking = true;
        lastClickTime = Time.time;

        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("Attack");

        if (audioSource != null && swingSound != null)
        {
            audioSource.PlayOneShot(swingSound);
        }
    }

    // 2. NUEVA FUNCIÓN: Conéctala al Animation Event en el frame de impacto
    public void PerformHitDetection()
    {
        // Detectamos enemigos en el frame exacto de la animación
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Nota: Asegúrate de que el nombre de la clase sea BoosHealth o BossHealth según tu script
            BoosHealth enemyHealth = enemy.GetComponent<BoosHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform.position);

                // Sacudida de cámara opcional al golpear
                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                if (gm != null) gm.SacudirCamara();
            }
        }

        // Avanzamos el combo aquí, justo cuando el golpe es efectivo
        comboStep++;
        if (comboStep > 2) comboStep = 0;
    }

    public void ResetCombo()
    {
        comboStep = 0;
        if (anim != null) anim.SetInteger("ComboStep", 0);
        isAttacking = false;
    }

    // Funciones para llamar desde el final de las animaciones
    public void EndAttack() { isAttacking = false; }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}