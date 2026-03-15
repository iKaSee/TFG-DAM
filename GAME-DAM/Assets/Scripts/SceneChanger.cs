using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneChanger : MonoBehaviour
{
    [Header("Configuración")]
    public string Escena_Boss; 
    public bool requiereTecla = true; 

    private bool jugadorCerca = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            
            if (!requiereTecla) CambiarEscena();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    void Update()
    {
        if (jugadorCerca && requiereTecla && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.W)))
        {
            CambiarEscena();
        }
    }

    void CambiarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Escena_Boss);
    }
}