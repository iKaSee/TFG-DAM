using System.Collections;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [SerializeField] private CanvasGroup panelFade;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (panelFade != null)
        {
            panelFade.alpha = 0f;
            panelFade.gameObject.SetActive(true);
        }
    }

    public void FadeOut(float duracion)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, duracion));
    }

    public void FadeIn(float duracion)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, duracion));
    }

    private IEnumerator FadeRoutine(float desde, float hasta, float duracion)
    {
        float tiempo = 0f;
        panelFade.alpha = desde;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            panelFade.alpha = Mathf.Lerp(desde, hasta, tiempo / duracion);
            yield return new WaitForEndOfFrame();
        }

        panelFade.alpha = hasta;
    }
}
