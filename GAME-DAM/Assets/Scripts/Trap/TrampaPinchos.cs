using UnityEngine;

public class TrampaEfecto : MonoBehaviour
{
    public int dañoInstantaneo = 20;
    public bool aplicaVeneno = false;

    // Cambiamos a OnTriggerStay2D para que si Jotem se queda encima, le duela sí o sí
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Esto saldrá en la consola SIEMPRE que algo toque los pinchos
        Debug.Log("COLISIÓN DETECTADA CON: " + collision.gameObject.name + " en capa: " + LayerMask.LayerToName(collision.gameObject.layer));

        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                if (aplicaVeneno)
                {
                    health.AplicarVeneno(2, 5, 1f);
                }
                else
                {
                    health.TakeDamage(dañoInstantaneo);
                }

                // Empuje para que no se quede pegado
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5.2f);
                }
            }
        }
    }
}