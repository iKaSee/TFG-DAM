using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 
using UnityEngine.UI; 

public class SceneChanger : MonoBehaviour
{
    [Header("Configuración")]
    public string Escena_Boss; 
    public bool requiereTecla = true; 
    public float tiempoAnimacion = 1.5f; 

    [Header("Transición (Fade)")]
    public Image panelFade; 
    private bool jugadorCerca = false;
    private bool estaEntrando = false; 
    private GameObject jugador; 





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = collision.gameObject;
            
            if (!requiereTecla && !estaEntrando) 
            {
                StartCoroutine(SecuenciaEntrar());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !estaEntrando)
        {
            jugadorCerca = false;
            jugador = null;
        }
    }

    void Update()
    {
        if (jugadorCerca && requiereTecla && !estaEntrando && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.W)))
        {
            StartCoroutine(SecuenciaEntrar());
        }
    }

    IEnumerator SecuenciaEntrar()
    {
        estaEntrando = true; 
        
        if (jugador != null)
        {
            Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            Animator anim = jugador.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("EnterDoor");

           
             jugador.GetComponent<PlayerController>().enabled = false;
        }

        if (panelFade != null)
        {
            float timer = 0;
            Color colorPanel = panelFade.color;
            
            while (timer < tiempoAnimacion)
            {
                timer += Time.deltaTime;
                
                
                float progreso = timer / tiempoAnimacion;
                colorPanel.a = progreso; 
                panelFade.color = colorPanel;
                
                yield return null; 
            }
        }
        else
        {
            yield return new WaitForSeconds(tiempoAnimacion);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(Escena_Boss);
    }
}