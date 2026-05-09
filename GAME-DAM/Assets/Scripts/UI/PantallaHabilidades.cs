using UnityEngine;

public class PantallaHabilidades : MonoBehaviour
{
    [SerializeField] private GameObject canvasHabilidades;

    private bool esperandoTecla = false;

    public void MostrarPantalla()
    {
        canvasHabilidades.SetActive(true);
        Time.timeScale = 0f;
        esperandoTecla = true;
    }

    void Update()
    {
        if (esperandoTecla && Input.anyKeyDown)
        {
            esperandoTecla = false;
            canvasHabilidades.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
