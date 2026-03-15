using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class FlickerHoguera : MonoBehaviour
{
    private Light2D luz;
    public float intensidadMin = 0.8f;
    public float intensidadMax = 1.4f;
    public float velocidad = 0.15f;

    void Start()
    {
        luz = GetComponent<Light2D>();
    }

    void Update()
    {
        luz.intensity = Mathf.Lerp(luz.intensity, Random.Range(intensidadMin, intensidadMax), velocidad);
    }
}