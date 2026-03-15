using UnityEngine;
using System.Collections;

public class GestorEscena : MonoBehaviour
{
    public PlayerController jotem;
    public CanvasGroup fadeCanvasGroup;

    void Start()
    {
        if (jotem != null) jotem.ForzarEstadoTumbado();

      if (fadeCanvasGroup != null) StartCoroutine(HacerFadeOut());
    }

    IEnumerator HacerFadeOut()
    {
        fadeCanvasGroup.alpha = 1; 
        float duracion = 2f;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, tiempo / duracion);
            yield return null;
        }
        
        fadeCanvasGroup.gameObject.SetActive(false); 
    }
}