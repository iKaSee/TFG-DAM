using System.Collections;
using UnityEngine;

public class TriggerMusica : MonoBehaviour
{
    [SerializeField] private AudioSource musicaFuente;
    [SerializeField] private float duracionFade = 2f;
    [SerializeField] private float volumenObjetivo = 1f;

    private bool yaActivado;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaActivado || !other.CompareTag("Player")) return;

        yaActivado = true;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        musicaFuente.volume = 0f;
        musicaFuente.Play();

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            musicaFuente.volume = Mathf.Lerp(0f, volumenObjetivo, tiempo / duracionFade);
            yield return null;
        }
        musicaFuente.volume = volumenObjetivo;
    }
}
