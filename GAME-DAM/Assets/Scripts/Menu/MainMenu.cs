using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Función para cargar el nivel principal
    public void Jugar()
    {
        // "MainScene" debe ser el nombre exacto de tu escena de juego
        SceneManager.LoadScene("MainScene");
    }

    // Función para cerrar el juego (solo funciona en el .exe)
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}