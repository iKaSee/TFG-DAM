using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        // IMPORTANTE: El objeto debe estar ACTIVADO en la jerarquía,
        // pero desactivamos solo la IMAGEN para que no se vea al inicio.
        img.enabled = false;
    }

    public void Flash()
    {
        // Activamos el componente para que se vea
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        img.enabled = true;
        yield return new WaitForSeconds(0.15f);
        img.enabled = false;
    }
}