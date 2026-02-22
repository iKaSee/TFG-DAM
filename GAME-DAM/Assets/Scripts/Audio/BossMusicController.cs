using UnityEngine;
using System.Collections;

public class BossMusicController : MonoBehaviour
{
    [Header("Música del Boss")]
    public AudioSource musicaFase1;
    public AudioSource musicaFase2;

    [Header("Ajustes de Transición")]
    public float velocidadFade = 0.5f;

    public void EmpezarMusicaBoss()
    {
        if (musicaFase1 != null) StartCoroutine(FadeIn(musicaFase1));
    }

    public void CambiarAFase2()
    {
        StartCoroutine(FadeOut(musicaFase1));
        StartCoroutine(FadeIn(musicaFase2));
    }

    public void PararTodo()
    {
        if (musicaFase1 != null) StartCoroutine(FadeOut(musicaFase1));
        if (musicaFase2 != null) StartCoroutine(FadeOut(musicaFase2));
    }

    IEnumerator FadeIn(AudioSource source)
    {
        if (source == null) yield break;
        source.volume = 0;
        source.Play();
        while (source.volume < 1)
        {
            source.volume += Time.deltaTime * velocidadFade;
            yield return null;
        }
        source.volume = 1;
    }

    IEnumerator FadeOut(AudioSource source)
    {
        if (source == null || !source.isPlaying) yield break;
        while (source.volume > 0)
        {
            source.volume -= Time.deltaTime * velocidadFade;
            yield return null;
        }
        source.Stop();
    }
}