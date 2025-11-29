// --- En tu script HealthComponent.cs (Modificado) ---

using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int currentHealth = 3;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " ha recibido " + damage + " de daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha sido derrotado y ocultado.");

        // 🚨 CAMBIO CLAVE: Desactivamos (ocultamos) el objeto en lugar de destruirlo.
        gameObject.SetActive(false);

        // Opcional: Si tienes una animación de muerte, la podrías activar
        // antes de esta línea, y luego programar la desactivación
        // usando un Animation Event al final de la animación de muerte.
    }
}