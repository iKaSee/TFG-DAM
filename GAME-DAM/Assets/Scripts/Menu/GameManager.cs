using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Añadimos esto para el texto
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public Image fondoNegro;
    public TextMeshProUGUI textoGameOver; // <--- ARRASTRA AQUÍ EL TEXTO DE "GAME OVER"
    public GameObject mensajeReintentar; // El que parpadea

    [Header("Efectos de Reaparición")]
    public Image flashBlanco; // <--- ARRASTRA AQUÍ LA IMAGEN BLANCA NUEVA

    [Header("Ajustes de Tiempo")]
    public float duracionMuerteSegundos = 30f; // <--- PON AQUÍ LOS SEGUNDOS QUE DURA TU ANIMACIÓN

    [Header("Ajustes de Vibración Cámara")]
    public Transform camaraPrincipal; // Arrastra aquí la cámara
    public float duracionVibracion = 0.1f;
    public float intensidadVibracion = 0.08f;

    public CanvasGroup fundidoCanvasGroup;
    private bool isGameOver = false;
    private bool puedeReiniciar = false;

    [Header("Economía")]
    public int oroTotal = 0; // Guardará las monedas
    public TextMeshProUGUI textoOro;



    // --- VARIABLE PARA RESETEAR TODO AL ABRIR EL JUEGO ---
    private static bool primeraVezQueCarga = true;

    void Awake()
    {
        if (instance == null) instance = this;

        // Si es la primera vez que abres el juego, borramos cualquier checkpoint viejo
        if (primeraVezQueCarga)
        {
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
            PlayerPrefs.Save();
            primeraVezQueCarga = false; // Marcamos que ya no es la primera carga
        }
    }

    void Start()
    {
        // --- LÓGICA DE REPOSICIONAMIENTO POR CHECKPOINT ---
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Movemos al jugador a la posición guardada
                player.transform.position = new Vector3(x, y, player.transform.position.z);

                // LIMPIAMOS LA OSCURIDAD SI EXISTE (Para que no reaparezca oscuro en el bosque)
               

                // LANZAMOS EL FLASH BLANCO
                StartCoroutine(EfectoFlashReaparecer());
            }
        }
        // --------------------------------------------------

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

    // --- FUNCIÓN PARA EL FLASH DE REAPARICIÓN ---
    IEnumerator EfectoFlashReaparecer()
    {
        if (flashBlanco == null) yield break;

        Color c = flashBlanco.color;
        c.a = 1f; // Aparece de golpe
        flashBlanco.color = c;

        float tiempo = 0;
        while (tiempo < 0.5f) // Se desvanece en medio segundo
        {
            tiempo += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, tiempo / 0.5f);
            flashBlanco.color = c;
            yield return null;
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


    public void TerminarJuego()
    {
        StartCoroutine(EsperarYCambiarEscena());
    }

    IEnumerator EsperarYCambiarEscena()
    {
        Debug.Log("Boss derrotado. Esperando para el fundido...");
        yield return new WaitForSeconds(6f); // Esperamos 6 segundos de los 10

        // Empezamos el fundido los últimos 4 segundos
        float duracionFade = 4f;
        float tiempo = 0;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            if (fundidoCanvasGroup != null)
                fundidoCanvasGroup.alpha = tiempo / duracionFade;
            yield return null;
        }

        SceneManager.LoadScene("EscenaFinal");
    }

    public void SumarOro(int cantidad)
    {
        oroTotal += cantidad;
        ActualizarInterfazOro();
    }

    void ActualizarInterfazOro()
    {
        if (textoOro != null)
        {
            textoOro.text = oroTotal.ToString();

            // Esto hace que el texto crezca y vuelva a su tamaño original
            StopCoroutine("AnimarTextoOro");
            StartCoroutine(AnimarTextoOro());
        }
    }

    IEnumerator AnimarTextoOro()
    {
        float duracion = 0.2f;
        Vector3 escalaOriginal = Vector3.one;
        Vector3 escalaGrande = new Vector3(1.3f, 1.3f, 1f);

        // Crece
        float tiempo = 0;
        while (tiempo < duracion / 2)
        {
            tiempo += Time.deltaTime;
            textoOro.transform.localScale = Vector3.Lerp(escalaOriginal, escalaGrande, tiempo / (duracion / 2));
            yield return null;
        }

        // Vuelve al tamaño normal
        tiempo = 0;
        while (tiempo < duracion / 2)
        {
            tiempo += Time.deltaTime;
            textoOro.transform.localScale = Vector3.Lerp(escalaGrande, escalaOriginal, tiempo / (duracion / 2));
            yield return null;
        }

        textoOro.transform.localScale = escalaOriginal;
    }



}