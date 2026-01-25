using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Añadimos esto para el texto
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Image fondoNegro;
    public TextMeshProUGUI textoGameOver; // <--- ARRASTRA AQUÍ EL TEXTO DE "GAME OVER"
    public GameObject mensajeReintentar; // El que parpadea

    [Header("Ajustes de Tiempo")]
    public float duracionMuerteSegundos = 30f; // <--- PON AQUÍ LOS SEGUNDOS QUE DURA TU ANIMACIÓN

    [Header("Ajustes de Vibración Cámara")]
    public Transform camaraPrincipal; // Arrastra aquí la cámara
    public float duracionVibracion = 0.1f;
    public float intensidadVibracion = 0.08f;

    private bool isGameOver = false;
    private bool puedeReiniciar = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        if (mensajeReintentar != null) mensajeReintentar.SetActive(false);

        // Ponemos el texto de Game Over invisible al inicio
        if (textoGameOver != null)
        {
            Color tc = textoGameOver.color;
            tc.a = 0;
            textoGameOver.color = tc;
        }

        Time.timeScale = 1f;
        isGameOver = false;
        puedeReiniciar = false;
    }

    void Update()
    {
        if (isGameOver && puedeReiniciar && Input.anyKeyDown)
        {
            RestartGame();
        }
    }

    // --- FUNCIÓN PARA LLAMAR DESDE EL COMBATE ---
    public void SacudirCamara()
    {
        if (camaraPrincipal != null)
        {
            StartCoroutine(ProcesoVibracion());
        }
    }

    IEnumerator ProcesoVibracion()
    {
        float tiempoPasado = 0f;
        float zOriginal = camaraPrincipal.localPosition.z;

        while (tiempoPasado < duracionVibracion)
        {
            // Vibración pura de izquierda a derecha (Eje X)
            float x = Random.Range(-1f, 1f) * intensidadVibracion;
            camaraPrincipal.localPosition = new Vector3(x, 0, zOriginal);

            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        camaraPrincipal.localPosition = new Vector3(0, 0, zOriginal);
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        StartCoroutine(EfectoMuerte());
    }

    IEnumerator EfectoMuerte()
    {
        float tiempoPasado = 0;
        Color cFondo = fondoNegro.color;
        Color cTexto = textoGameOver.color;

        // Este bucle durará exactamente los segundos que hayas puesto en duracionMuerteSegundos
        while (tiempoPasado < duracionMuerteSegundos)
        {
            tiempoPasado += Time.unscaledDeltaTime;

            // Calculamos el progreso (de 0 a 1) basado en el tiempo
            float progreso = tiempoPasado / duracionMuerteSegundos;

            // Aplicamos al fondo
            cFondo.a = progreso;
            fondoNegro.color = cFondo;

            // Aplicamos al texto de GAME OVER
            if (textoGameOver != null)
            {
                cTexto.a = progreso;
                textoGameOver.color = cTexto;
            }

            yield return null;
        }

        // Nos aseguramos de que al final sea 1 total
        cFondo.a = 1;
        fondoNegro.color = cFondo;
        if (textoGameOver != null) { cTexto.a = 1; textoGameOver.color = cTexto; }

        // Al terminar el fundido, aparece el mensaje de "Pulsa tecla"
        if (mensajeReintentar != null) mensajeReintentar.SetActive(true);

        puedeReiniciar = true;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}