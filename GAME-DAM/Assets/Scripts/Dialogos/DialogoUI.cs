using TMPro;
using UnityEngine;

public class DialogoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoDialogo;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void MostrarLinea(string linea)
    {
        gameObject.SetActive(true);
        textoDialogo.text = linea;
    }

    public void Ocultar()
    {
        gameObject.SetActive(false);
    }
}
