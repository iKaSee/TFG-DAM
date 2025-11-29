using UnityEngine;

public class AIChase : MonoBehaviour
{
    // --- Variables P�blicas para Configuraci�n ---

    [Header("Movimiento")]
    public float moveSpeed = 3f; // Velocidad de movimiento del enemigo

    [Header("Detecci�n")]
    public float chaseRange = 8f; // Radio para empezar a perseguir

    // --- Variables Privadas ---
    private Transform target;       // Referencia al Transform del jugador
    private Rigidbody2D rb;         // Referencia al Rigidbody2D

    // --- M�todos ---

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Intenta encontrar el objeto del jugador por su etiqueta "Player"
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        // 1. Verificar si tenemos un objetivo
        if (target == null) return;

        // 2. Calcular la distancia al objetivo
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // 3. L�gica de Detecci�n
        if (distanceToTarget <= chaseRange)
        {
            // El jugador est� dentro del rango
            ChaseTarget();
        }
        else
        {
            // El jugador est� fuera del rango
            rb.linearVelocity = Vector2.zero; // Detener al enemigo
        }
    }

    private void ChaseTarget()
    {
        // 1. Calcular la direcci�n
        // Restamos la posici�n del objetivo menos la posici�n actual para obtener el vector de direcci�n
        Vector2 direction = (target.position - transform.position).normalized;

        // 2. Mover el Rigidbody2D
        // Usamos la velocidad para mover al enemigo en la direcci�n calculada
        rb.linearVelocity = direction * moveSpeed;

        // 3. Rotar (Opcional: Si quieres que el sprite mire al jugador)
        RotateTowardsTarget(direction);
    }

    private void RotateTowardsTarget(Vector2 direction)
    {
        // Esta l�gica asume que el eje "derecha" (right) del sprite es su frente (forward).
        // Calcula el �ngulo en radianes usando Atan2
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotaci�n a nuestro GameObject
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Opcional: Para visualizar el rango de persecuci�n en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}