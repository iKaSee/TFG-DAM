using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HogueraBoss : MonoBehaviour
{
    [SerializeField] private GameObject promptTecla;
    [SerializeField] private CanvasGroup fadePropio;

    private PlayerController playerController;
    private bool jugadorCerca = false;
    private bool yaActivada = false;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (jugadorCerca && !yaActivada && Input.GetKeyDown(KeyCode.G))
        {
            yaActivada = true;
            IniciarSecuencia();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (promptTecla != null) promptTecla.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (promptTecla != null) promptTecla.SetActive(false);
        }
    }

    public void IniciarSecuencia()
    {
        if (promptTecla != null) promptTecla.SetActive(false);
        StartCoroutine(SecuenciaDescanso());
    }

    private IEnumerator SecuenciaDescanso()
    {
        playerController?.SetControlsEnabled(false);

        yield return new WaitForSeconds(0.5f);

        float duracion = 1.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            fadePropio.alpha = Mathf.Clamp01(tiempo / duracion);
            yield return null;
        }

        fadePropio.alpha = 1f;

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Pueblo");
    }
}
