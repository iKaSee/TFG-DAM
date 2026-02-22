using UnityEngine;
using UnityEngine.UI;
using TMPro; // IMPRESCINDIBLE

public class BossHealthBarController : MonoBehaviour
{
    [Header("Asignaciones")]
    public GameObject panelBarra;
    public Image barraRoja;

    // Usamos TMP_Text para que acepte cualquier objeto de TextMeshPro
    public TMP_Text nombreBoss;

    public void ActivarBarra(string nombre)
    {
        if (panelBarra != null) panelBarra.SetActive(true);
        if (nombreBoss != null) nombreBoss.text = nombre;
        if (barraRoja != null) barraRoja.fillAmount = 1f;
    }

    public void ActualizarBarra(float vidaActual, float vidaMax)
    {
        if (barraRoja != null)
        {
            barraRoja.fillAmount = vidaActual / vidaMax;
        }
    }

    public void DesactivarBarra()
    {
        if (panelBarra != null) panelBarra.SetActive(false);
    }
}