using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image relleno;
    [SerializeField] private GameObject canvasBoss;

    void Awake()
    {
        if (canvasBoss != null) canvasBoss.SetActive(false);
    }

    public void IniciarBarra(int maxHealth)
    {
        if (canvasBoss != null) canvasBoss.SetActive(true);
        if (relleno != null) relleno.fillAmount = 1f;
    }

    public void ActualizarBarra(int currentHealth, int maxHealth)
    {
        if (relleno == null || maxHealth <= 0) return;
        relleno.fillAmount = (float)currentHealth / maxHealth;
    }

    public void OcultarBarra()
    {
        if (canvasBoss != null) canvasBoss.SetActive(false);
    }

    public bool EstaActiva()
    {
        return canvasBoss != null && canvasBoss.activeSelf;
    }
}
