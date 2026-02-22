using UnityEngine;




public class AudioManagerSimple : MonoBehaviour
{

    [Header("Sonido de Impacto")]
    public AudioClip sonidoMazoSuelo; // Aquí arrastra el sonido del mazo
    public float volumenImpacto = 1.0f;


    public AudioSource fuenteAudio;
    public AudioClip[] pasos; // Una lista por si tienes varios sonidos de pasos

    // Esta función la llamará la animación
    public void PlayFootstep()
    {
        if (fuenteAudio != null && pasos.Length > 0)
        {
            // Elegimos un paso al azar de la lista para que no sea repetitivo
            int indiceAleatorio = Random.Range(0, pasos.Length);
            fuenteAudio.PlayOneShot(pasos[indiceAleatorio]);
        }
    }

    public void PlayMazoImpacto()
    {
        if (fuenteAudio != null && sonidoMazoSuelo != null)
        {
            // Forzamos el pitch a 1 para que el impacto siempre suene potente y no agudo
            fuenteAudio.pitch = 1.0f;
            fuenteAudio.PlayOneShot(sonidoMazoSuelo, volumenImpacto);

            Debug.Log("¡BUM! El mazo ha impactado.");
        }
    }


}