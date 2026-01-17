using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private Transform padreReal;
    private Vector3 offset;

    void Start()
    {
        padreReal = transform.parent;
        // Guardamos la distancia inicial respecto al murciélago
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        if (padreReal == null || Camera.main == null) return;

        // 1. POSICIÓN: Forzamos la posición para que siga al padre
        // pero lo hacemos usando la posición global para que no le afecte el giro
        transform.position = padreReal.TransformPoint(offset);

        // 2. ROTACIÓN: Mirar siempre a cámara (Billboard)
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        // 3. ESCALA: Forzamos una escala global fija
        // Esto ignora totalmente si el padre tiene -1 o 500.
        transform.localScale = Vector3.one * 0.01f;
    }

    public void ConfigurarBarra(float actual, float max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = actual;
    }
}