using System.Collections;
using Cinemachine;
using UnityEngine;

public class GestorPueblo : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private AudioSource audioRocas;
    [SerializeField] private Transform rocas;
    [SerializeField] private CinemachineVirtualCamera vcamJotem;
    [SerializeField] private CinemachineVirtualCamera vcamRocas;

    private bool secuenciaIniciada;

    void Start()
    {
        Debug.Log("GestorPueblo Start ejecutado");
        fadeCanvasGroup.alpha = 1f;
        audioRocas.Play();
        vcamRocas.Priority = 20;
        vcamJotem.Priority = 0;
    }

    void Update()
    {
        if (!secuenciaIniciada && Input.anyKeyDown)
        {
            secuenciaIniciada = true;
            StartCoroutine(SecuenciaInicio());
        }
    }

    private IEnumerator SecuenciaInicio()
    {
        // Fade in: de negro a transparente
        float duracion = 2f;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;

        // Mantiene la cámara fija sobre las rocas un momento
        yield return new WaitForSeconds(2f);

        // Devuelve el control a la vcam que sigue a Jotem
        vcamJotem.Priority = 10;
        vcamRocas.Priority = 0;
    }
}
