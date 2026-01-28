using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZonaOscura : MonoBehaviour
{
    public Image imagenOscuridad;
    public float alphaMaximo = 0.8f;

    // --- NUEVA FUNCIÓN PARA EL GAMEMANAGER ---
    public void LimpiarOscuridadAlInstante()
    {
        StopAllCoroutines();
        if (imagenOscuridad != null)
        {
            Color c = imagenOscuridad.color;
            c.a = 0f;
            imagenOscuridad.color = c;
        }
    }
    // -----------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ALGO HA TOCADO EL TRIGGER: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("¡ES EL JUGADOR!");
            StopAllCoroutines();
            StartCoroutine(CambiarAlpha(alphaMaximo));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(CambiarAlpha(0f));
        }
    }

    IEnumerator CambiarAlpha(float objetivo)
    {
        if (imagenOscuridad == null) yield break;

        Color c = imagenOscuridad.color;
        float actual = c.a;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            c.a = Mathf.Lerp(actual, objetivo, t);
            imagenOscuridad.color = c;
            yield return null;
        }
    }
}