using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarMenu : MonoBehaviour
{
    // Nombre exacto de tu escena de menú
    public string nombreEscenaMenu = "MenuPrincipal";

    void Update()
    {
        // OPCIÓN B: Si el ratón falla, pulsa CUALQUIER TECLA para volver
        if (Input.anyKeyDown)
        {
            Volver();
        }
    }

    // OPCIÓN A: Esta función se llamará desde el botón
    public void Volver()
    {
        Debug.Log("Cargando escena: " + nombreEscenaMenu);
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}