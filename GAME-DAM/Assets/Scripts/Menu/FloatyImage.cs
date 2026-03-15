using UnityEngine;

public class FloatyImage : MonoBehaviour
{
    public float amplitud = 15f; // Cuántos pixels se mueve 
    public float velocidad = 2f; // Qué tan rápido
    private RectTransform rectTransform;
    private Vector2 posInicial;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            posInicial = rectTransform.anchoredPosition;
        }
        else
        {
            Debug.LogError("Este script necesita un RectTransform (estar en un objeto de UI).");
            enabled = false; 
        }
    }

    void Update()
    {
        
        float nuevaY = Mathf.Sin(Time.time * velocidad) * amplitud;
        rectTransform.anchoredPosition = posInicial + new Vector2(0, nuevaY);
    }
}