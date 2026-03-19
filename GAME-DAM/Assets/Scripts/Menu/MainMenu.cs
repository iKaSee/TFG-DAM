using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    [Header("Sonidos")]
    public AudioSource audioNavegacion;
    public AudioSource audioConfirmacion;

    [Header("Referencias UI")]
    public CanvasGroup grupoBotones; 
    public Camera camaraMenu;

    [Header("Ajustes de Zoom")]
    public Vector3 posicionJotem; 
    public float zoomFinal = 3f;  
    
    [Header("Efecto Hoguera")]
    public UnityEngine.Rendering.Universal.Light2D luzHoguera;



    public EfectoFade scriptFade;   

    public void BotonJugar(string Mina)
    {
        StartCoroutine(SecuenciaEmpezar(Mina));
    }

    public void SonidoResaltar()
    {
        audioNavegacion.Play();
    }

    IEnumerator SecuenciaEmpezar(string escena)
{
    audioConfirmacion.Play();

    float tiempo = 0;
    float duracion = 3; 
    
    Vector3 posInicialCamara = camaraMenu.transform.position;
    float sizeInicial = camaraMenu.orthographicSize;
    Vector3 posDestino = new Vector3(posicionJotem.x, posicionJotem.y, -10f);

    float intensidadInicial = luzHoguera != null ? luzHoguera.intensity : 0;        

    if (luzHoguera.GetComponent<FlickerHoguera>() != null) 
    luzHoguera.GetComponent<FlickerHoguera>().enabled = false;

    while (tiempo < duracion)
    {
        tiempo += Time.deltaTime;
        float t = tiempo / duracion;
        float progresoSuave = Mathf.SmoothStep(0, 1, t);

        if (luzHoguera != null)
        {
            // La luz baja de su intensidad inicial a 0
            luzHoguera.intensity = Mathf.Lerp(intensidadInicial, 0, progresoSuave);
        }

        if(grupoBotones != null) grupoBotones.alpha = 1 - t;
        
        if(scriptFade != null) scriptFade.EstablecerAlpha(t);

        camaraMenu.transform.position = Vector3.Lerp(posInicialCamara, posDestino, t);
        camaraMenu.orthographicSize = Mathf.Lerp(sizeInicial, zoomFinal, t);
        
        yield return null;
    }
    if (luzHoguera != null) luzHoguera.intensity = 0;

    SceneManager.LoadScene(escena);
}

    public void SalirDelJuego()
{
    Debug.Log("Saliendo del juego..."); 
    Application.Quit(); 
}

}