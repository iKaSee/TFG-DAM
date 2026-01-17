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
    public float velocidadOscurecimiento = 0.7f; // Un poco más lento queda mejor

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

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        StartCoroutine(EfectoMuerte());
    }

    IEnumerator EfectoMuerte()
    {
        float alpha = 0;
        Color cFondo = fondoNegro.color;
        Color cTexto = textoGameOver.color;

        while (alpha < 1)
        {
            // Aumentamos el alpha gradualmente
            alpha += Time.unscaledDeltaTime * velocidadOscurecimiento;

            // Aplicamos al fondo
            cFondo.a = alpha;
            fondoNegro.color = cFondo;

            // Aplicamos al texto de GAME OVER
            if (textoGameOver != null)
            {
                cTexto.a = alpha;
                textoGameOver.color = cTexto;
            }

            yield return null;
        }

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