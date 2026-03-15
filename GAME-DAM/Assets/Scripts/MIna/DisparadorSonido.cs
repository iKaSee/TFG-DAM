using UnityEngine;

public class DisparadorSonido : MonoBehaviour
{public AudioSource sourceSonido; 
    private bool yaSeActivo = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaSeActivo)
        {
            ActivarEvento();
        }
    }

    void ActivarEvento()
    {
        yaSeActivo = true; 
        
        if (sourceSonido != null)
        {
            sourceSonido.Play();
            Debug.Log("Sonido de evento activado en la mina.");
        }

        // Destroy(gameObject, sourceSonido.clip.length); 
    }
}