using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // Cambiado a la tecla F
            if (Input.GetKeyDown(KeyCode.F))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // El trigger debe ser instantáneo
        anim.SetTrigger("Attack");
    }

    // Mantén estas funciones para evitar los errores de consola que vimos antes
    public void EndAttack() { }
    public void DisableHitboxAndEndAttack() { }
}