using UnityEngine;

public class TriggerBosque : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el objeto del Parallax y lo activamos
            FindObjectOfType<ParallaxMovement>().ActivarParallax();

            // Opcional: Destruir el trigger para que no se active más veces
            Destroy(gameObject);
        }
    }
}