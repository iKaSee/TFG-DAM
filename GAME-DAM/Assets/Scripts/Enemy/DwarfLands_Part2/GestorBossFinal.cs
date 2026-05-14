using UnityEngine;
using System.Collections;
using Cinemachine;

public class GestorBossFinal : MonoBehaviour
{
    [SerializeField] private GameObject pyrethAsesino;
    [SerializeField] private GameObject pyrethTanque;
    [SerializeField] private Transform spawnJotem;
    [SerializeField] private DialogoUI dialogoUI;
    [SerializeField] private string[] lineasTanque;
    [SerializeField] private CinemachineVirtualCamera vcamPrincipal;
    [SerializeField] private CinemachineVirtualCamera vcamBoss;
    [SerializeField] private float duracionFade = 1.5f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private bool fase2Iniciada = false;
    private PlayerController playerController;

    void Start()
    {
        playerController = Object.FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (fase2Iniciada) return;

        if (pyrethAsesino == null || !pyrethAsesino.activeInHierarchy)
        {
            fase2Iniciada = true;
            StartCoroutine(TransicionFase2());
        }
    }

    IEnumerator TransicionFase2()
    {
        if (playerController != null) playerController.SetControlsEnabled(false);

        yield return StartCoroutine(Fade(0f, 1f, duracionFade));

        if (playerController != null && spawnJotem != null)
        {
            playerController.transform.position = spawnJotem.position;
        }

        if (pyrethTanque != null) pyrethTanque.SetActive(true);

        if (vcamBoss != null) vcamBoss.Priority = 20;
        if (vcamPrincipal != null) vcamPrincipal.Priority = 0;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(1f, 0f, duracionFade));

        if (GameManager.instance != null) GameManager.instance.SacudirCamara();

        yield return new WaitForSeconds(1f);

        if (dialogoUI != null && lineasTanque != null)
        {
            for (int i = 0; i < lineasTanque.Length; i++)
            {
                dialogoUI.MostrarLinea(lineasTanque[i]);
                yield return new WaitForSeconds(1.5f);
            }
            dialogoUI.Ocultar();
        }

        if (vcamPrincipal != null) vcamPrincipal.Priority = 10;
        if (vcamBoss != null) vcamBoss.Priority = 0;

        if (playerController != null) playerController.SetControlsEnabled(true);
    }

    IEnumerator Fade(float desde, float hasta, float duracion)
    {
        if (fadeCanvasGroup == null)
        {
            yield return new WaitForSeconds(duracion);
            yield break;
        }

        float t = 0f;
        fadeCanvasGroup.alpha = desde;
        while (t < duracion)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(desde, hasta, t / duracion);
            yield return null;
        }
        fadeCanvasGroup.alpha = hasta;
    }
}
