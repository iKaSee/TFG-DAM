using UnityEngine;
using UnityEngine.UI;

public class EfectoFade : MonoBehaviour
{
    private Image imagen;

    void Awake()
    {
        imagen = GetComponent<Image>();
        Color c = imagen.color;
        c.a = 0;
        imagen.color = c;
    }

    public void EstablecerAlpha(float valor)
    {
        Color c = imagen.color;
        c.a = valor;
        imagen.color = c;
    }
}