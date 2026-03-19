using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuNavegacion : MonoBehaviour
{
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}