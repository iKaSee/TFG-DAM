using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoFade = 1.5f;

    private Image panelOscuro;

    void Start()
    {
        panelOscuro = GetComponent<Image>();
        
        if (panelOscuro != null)
        {
            StartCoroutine(EfectoAclarar());
        }
    }

    IEnumerator EfectoAclarar()
    {
        float timer = tiempoFade;
        Color colorPanel = panelOscuro.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            float progreso = timer / tiempoFade;
            colorPanel.a = progreso; 
            panelOscuro.color = colorPanel;
            
            yield return null; 
        }

        gameObject.SetActive(false);
    }
}