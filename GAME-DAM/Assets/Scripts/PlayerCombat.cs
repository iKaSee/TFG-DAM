using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;

    [Header("Configuración de Ataque")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    // ESTA VARIABLE DEBE SER PÚBLICA PARA QUE EL CONTROLLER LA VEA
    public bool isAttacking;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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
        anim.SetTrigger("Attack");
    }

    // --- ESTAS FUNCIONES QUITAN LOS ERRORES ROJOS DE TU CONSOLA ---
    // Se deben llamar mediante Animation Events al final de tu animación
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void DisableHitboxAndEndAttack()
    {
        isAttacking = false;
    }
}