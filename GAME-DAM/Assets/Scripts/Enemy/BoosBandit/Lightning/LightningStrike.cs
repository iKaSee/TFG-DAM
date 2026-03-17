using UnityEngine;
using System.Collections;

public class LightningStrike : MonoBehaviour
{
    [Header("Configuración")]
    public float warningTime = 1f; // Tiempo que el jugador tiene para esquivar
    public int damage = 25;

    [Header("Referencias (Asignar en Inspector)")]
    public GameObject warningIndicador; 
    public GameObject rayoVisual;       
    public Collider2D damageCollider;   

    void Start()
    {
        StartCoroutine(SecuenciaPilarDeLuz());
    }

    IEnumerator SecuenciaPilarDeLuz()
    {
        warningIndicador.SetActive(true);
        rayoVisual.SetActive(false);
        damageCollider.enabled = false;

        // Esperamos a que el jugador reaccione
        yield return new WaitForSeconds(warningTime);

        warningIndicador.SetActive(false);
        rayoVisual.SetActive(true);
        damageCollider.enabled = true;

        Vector3 escalaOriginal = rayoVisual.transform.localScale;
        
        rayoVisual.transform.localScale = new Vector3(0.1f, escalaOriginal.y, escalaOriginal.z);

        float tiempoEnsanchar = 0.1f;
        float timer = 0;
        while (timer < tiempoEnsanchar)
        {
            timer += Time.deltaTime;
            float progreso = timer / tiempoEnsanchar;
            
            // Crece de 0.1 de ancho a 1.5 de ancho de golpe
            float anchoActual = Mathf.Lerp(0.1f, 1.5f, progreso);
            rayoVisual.transform.localScale = new Vector3(anchoActual, escalaOriginal.y, escalaOriginal.z);
            yield return null;
        }

        damageCollider.enabled = false;

        SpriteRenderer sr = rayoVisual.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            while (c.a > 0)
            {
                c.a -= Time.deltaTime * 3f; 
                
                // Mientras desaparece, lo hacemos más fino otra vez
                float anchoDesvanecido = rayoVisual.transform.localScale.x - (Time.deltaTime * 2f);
                rayoVisual.transform.localScale = new Vector3(anchoDesvanecido, escalaOriginal.y, escalaOriginal.z);
                
                sr.color = c;
                yield return null; 
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) 
            {
                ph.TakeDamage(damage);
            }
        }
    }
}