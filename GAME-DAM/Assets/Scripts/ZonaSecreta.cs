using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class ZonaSecreta : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap tilemap;

    [Header("Configuración")]
    public float velocidadFade = 4f;
    public float alphaOculto = 0.2f;

    void Awake()
    {
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
        if (tilemap == null) tilemap = GetComponentInParent<Tilemap>();
        if (tilemap == null && transform.parent != null)
            tilemap = transform.parent.GetComponentInChildren<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeTilemap(alphaOculto));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeTilemap(1f));
        }
    }

    IEnumerator FadeTilemap(float targetAlpha)
    {
        if (tilemap == null) yield break;

        Color colorActual = tilemap.color;
        while (!Mathf.Approximately(colorActual.a, targetAlpha))
        {
            colorActual.a = Mathf.MoveTowards(colorActual.a, targetAlpha, velocidadFade * Time.deltaTime);
            tilemap.color = colorActual;
            yield return null;
        }
    }
}