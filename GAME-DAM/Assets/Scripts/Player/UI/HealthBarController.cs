using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image imagenBarra; // Arrastra aquí el componente Image de tu UI
    public Sprite[] spritesVida; // Aquí irán tus 9 sprites (del 0 al 8)

    // Esta función la llama el script PlayerHealth
    public void SetVidaMaxima(int vidaMax)
    {
        ActualizarVida(vidaMax); // Al empezar, que se vea llena
    }

    public void ActualizarVida(int vidaActual)
    {
        if (spritesVida.Length == 0) return;

        // Calculamos el índice basándonos en 100 de vida y 9 sprites
        // Si vida es 100 -> índice 0 (lleno)
        // Si vida es 0 -> índice 8 (vacío)

        float porcentajeVida = (float)vidaActual / 100f; // Asumiendo que 100 es el máximo
        int indice = 8 - Mathf.FloorToInt(porcentajeVida * 8);

        // Aseguramos que el índice no se salga de la lista (0 a 8)
        indice = Mathf.Clamp(indice, 0, spritesVida.Length - 1);

        if (imagenBarra != null)
        {
            imagenBarra.sprite = spritesVida[indice];
        }
    }
}