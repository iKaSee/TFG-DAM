using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para acceder a las luces 2D

public class LightFlicker : MonoBehaviour
{
    private Light2D lightSource;
    public float minIntensity = 1.0f;
    public float maxIntensity = 1.3f;

    void Awake() { lightSource = GetComponent<Light2D>(); }

    void Update()
    {
        // Cambia la intensidad aleatoriamente para simular una llama
        lightSource.intensity = Random.Range(minIntensity, maxIntensity);
    }
}