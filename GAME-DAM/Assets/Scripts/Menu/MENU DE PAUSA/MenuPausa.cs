using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioSource audioNavegacion;

    [Header("Referencias de Canvas")]
    public GameObject canvasPadre;
    public GameObject objetoMenuPausa; 
    public GameObject panelOpciones; 


    public static bool juegoPausado = false;


void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (panelOpciones.activeInHierarchy)
        {
            panelOpciones.SetActive(false);
            objetoMenuPausa.SetActive(true);
            return; 
        }

        if (juegoPausado)
        {
            Continuar();
        }
        else
        {
            Pausar();
        }
    }
}

   public void Continuar()
{
    objetoMenuPausa.SetActive(false);
    panelOpciones.SetActive(false);
    canvasPadre.SetActive(false); 
    
    Time.timeScale = 1f; 
    juegoPausado = false;
}
void Pausar()
{
    
    canvasPadre.SetActive(true); 
    objetoMenuPausa.SetActive(true); 
    panelOpciones.SetActive(false); 

    Time.timeScale = 0f; 
    juegoPausado = true;
}

    public void SonidoResaltar()
    {
        audioNavegacion.Play();
    }


    public void SalirAlMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}