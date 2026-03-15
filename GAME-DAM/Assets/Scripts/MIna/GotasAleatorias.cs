using UnityEngine;
using System.Collections;

public class GotasAleatorias : MonoBehaviour
{
    public AudioSource altavoz;
    public AudioClip sonidoGota; 
    
    [Header("Tiempos de espera")]
    public float esperaMin = 3f;
    public float esperaMax = 7f;

    void Start()
    {
        if (altavoz == null) altavoz = GetComponent<AudioSource>();
        
        // Empezamos la rutina de goteo
        StartCoroutine(RutinaGoteo());
    }

    IEnumerator RutinaGoteo()
    {
        while (true)
        {
            float tiempoEspera = Random.Range(esperaMin, esperaMax);
            yield return new WaitForSeconds(tiempoEspera);

            if (sonidoGota != null)
            {
                altavoz.pitch = Random.Range(0.8f, 1.2f);
                
                // Suena la gota
                altavoz.PlayOneShot(sonidoGota);
            }
        }
    }
}