using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Esta función la llamaremos desde PlayerHealth cada vez que recibas daño
    public void ActualizarVida(int vidaActual)
    {
        slider.value = vidaActual;
    }

    // Configura el máximo de vida al empezar
    public void SetVidaMaxima(int vidaMax)
    {
        slider.maxValue = vidaMax;
        slider.value = vidaMax;
    }
}