using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;

    [Header("Configuracion de Ataque")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public bool isAttacking;

    [Header("Deteccion de Daño")]
    public Transform attackPoint;    // Arrastra aqui el objeto AttackPoint
    public float attackRange = 0.5f; // El tamaño del circulo de golpe
    public LayerMask enemyLayers;    // Seleccionaremos la capa "Enemy"
    public int attackDamage = 40;    // El daño que harás

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swingSound; // El sonido de la espada en el aire


    [Header("Ajustes de Combo")]
    public int comboStep = 0;
    public float comboResetTime = 0.5f; // Tiempo máximo entre clics para seguir el combo
    private float lastClickTime;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Lógica para reiniciar el combo si pasa mucho tiempo sin atacar
        if (Time.time - lastClickTime > comboResetTime)
        {
            comboStep = 0;
            anim.SetInteger("ComboStep", 0);

            // Esta línea es el "seguro de vida": si el evento falla, 
            // el tiempo te devolverá el movimiento.
            isAttacking = false;
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

    void Attack()
    {
        isAttacking = true;

        // Guardamos el momento del clic para el temporizador
        lastClickTime = Time.time;

        // Avisamos al Animator en qué paso del combo estamos antes de disparar el Trigger
        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("Attack");

        // Esta linea crea un circulo invisible y guarda todo lo que sea "Enemy" dentro de el
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (audioSource != null && swingSound != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        // Por cada enemigo que hayamos golpeado...
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Pasamos el daño Y la posición actual del jugador (transform.position)
                enemyHealth.TakeDamage(attackDamage, transform.position);
            }
        }

        // Avanzamos el combo para el siguiente clic
        comboStep++;
        if (comboStep > 2) // Si ya hicimos el tercer ataque (0, 1, 2), volvemos a empezar
        {
            comboStep = 0;
        }
    }

    public void EndAttack() { isAttacking = false; }
    public void DisableHitboxAndEndAttack() { isAttacking = false; }

    // Dibuja el circulo rojo en la ventana Scene para ayudarte a configurar
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}