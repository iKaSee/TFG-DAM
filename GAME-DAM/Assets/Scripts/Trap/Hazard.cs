using UnityEngine;

public class HazardTilemap : MonoBehaviour
{
    [Header("Ajustes de Daño")]
    public int daño = 20;
    public float fuerzaEmpuje = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si es el jugador (Asegúrate de que Jotem tenga el Tag "Player")
        if (other.CompareTag("Player"))
        {
            // Intentamos obtener el componente de salud
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(daño);
            }

            // Efecto de retroceso (Knockback) para que no se quede pegado a los pinchos
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Limpiamos velocidad y empujamos hacia arriba
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaEmpuje, ForceMode2D.Impulse);
            }

            // Feedback visual: Sacudida de cámara (si tienes el GameManager configurado)
            // GameManager.instance.SacudirCamara(); 
        }
    }
}