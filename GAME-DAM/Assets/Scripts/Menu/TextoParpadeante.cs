using UnityEngine;
using TMPro;

public class TextoParpadeante : MonoBehaviour
{
    private TextMeshProUGUI texto;
    public float velocidad = 2.5f;

    void Awake() // Usamos Awake para que esté listo antes de activarse
    {
        texto = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Crea un ciclo de transparencia suave entre 0 y 1
        float alpha = (Mathf.Sin(Time.unscaledTime * velocidad) + 1) / 2;
        texto.alpha = alpha;
    }
}