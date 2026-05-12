using System.Collections;
using UnityEngine;

public class PantallaHabilidades : MonoBehaviour
{
    [SerializeField] private GameObject canvasHabilidades;

    private bool esperandoTecla = false;

    public void MostrarPantalla()
    {
        Debug.Log("MostrarPantalla llamado | canvas: " + (canvasHabilidades == null ? "NULL" : "OK"));
        canvasHabilidades.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(EsperarInput());
    }

    private IEnumerator EsperarInput()
    {
        yield return new WaitForSecondsRealtime(0.3f);
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
