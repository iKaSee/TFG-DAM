using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para controlar la Light2D
using UnityEngine.UI; // --- Necesario para actualizar la barrita del Slider al empezar ---

public class BrightnessController : MonoBehaviour
{
    [Header("Referencias")]
    // Ya no es public, el script la buscará solo
    private Light2D globalLight; 
    
    // --- Referencia al Slider para que no se quede visualmente a cero ---
    public Slider sliderBrillo;
    // -------------------------------------------------------------------------

    [Header("Límites de Brillo")]
    // Ajusta estos valores según lo que quede bien en tu juego
    public float intensidadMinima = 0.5f; 
    public float intensidadMaxima = 1.5f; // Lo he bajado un poco para que no queme la imagen

    // Ajuste de Color para no perder la atmósfera
    [Header("Ajustes de Color (Atmósfera)")]
    public Color colorOscuro = new Color(0.2f, 0.4f, 0.8f, 1f); // Un azul oscuro de ejemplo
    public Color colorClaro = new Color(0.7f, 0.8f, 1f, 1f);  // Un azul muy clarito/grisáceo

    void Start()
    {
        // El script busca la luz global de la escena actual automáticamente
        Light2D[] todasLasLuces = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (Light2D luz in todasLasLuces)
        {
            if (luz.lightType == Light2D.LightType.Global)
            {
                globalLight = luz;
                break; // En cuanto la encuentra, deja de buscar
            }
        }

        // --- Cargar el valor guardado ---
        // Si ya guardamos un valor antes, lo cargamos. Si es la primera vez que juega, ponemos 0.5 por defecto.
        float brilloGuardado = PlayerPrefs.GetFloat("BrilloGuardado", 0.2f);
        
        // Actualizamos la barrita visualmente en el menú
        if (sliderBrillo != null)
        {
            sliderBrillo.value = brilloGuardado;
        }

        // Aplicamos la luz de verdad
        CambiarBrillo(brilloGuardado);
        // ---------------------------------------
    }

    // Esta es la función que llamará el Slider de la UI cuando lo muevas
    public void CambiarBrillo(float valorSlider)
    {
        if (globalLight != null)
        {
            // 1. Cambiamos la intensidad
            float nuevaIntensidad = Mathf.Lerp(intensidadMinima, intensidadMaxima, valorSlider);
            globalLight.intensity = nuevaIntensidad;

            // 2. Transición suave de color
            // Color.Lerp hace exactamente lo mismo pero mezclando los dos colores
            Color nuevoColor = Color.Lerp(colorOscuro, colorClaro, valorSlider);
            globalLight.color = nuevoColor;

            // --- Guardar el valor en el ordenador del jugador ---
            PlayerPrefs.SetFloat("BrilloGuardado", valorSlider);
            PlayerPrefs.Save(); // Forzamos el guardado por seguridad
            // -----------------------------------------------------------
        }
        else
        {
            Debug.LogWarning("¡No se ha encontrado ninguna Global Light 2D en esta escena!");
        }
    }
}