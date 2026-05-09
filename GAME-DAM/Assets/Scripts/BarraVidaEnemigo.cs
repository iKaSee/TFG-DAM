using UnityEngine;
using UnityEngine.UI;

public class BarraVidaEnemigo : MonoBehaviour
{
    [SerializeField] private Image relleno;
    [SerializeField] private GameObject canvasBarra;

    void Awake()
    {
        if (canvasBarra != null) canvasBarra.SetActive(false);
    }

    public void ActualizarVida(float currentHealth, float maxHealth)
    {
        if (canvasBarra != null && !canvasBarra.activeSelf) canvasBarra.SetActive(true);

        float fillAmount = currentHealth / maxHealth;
        if (relleno != null) relleno.fillAmount = fillAmount;
    }

    public void OcultarBarra()
    {
        if (canvasBarra != null) canvasBarra.SetActive(false);
    }
}
