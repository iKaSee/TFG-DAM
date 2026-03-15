using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Ajustes de Pasos")]
    public AudioSource sourcePasos;
    public AudioClip[] clipsPasos; 
    [Range(0, 1)] public float volumenPasos = 0.5f;
    [Range(0.1f, 2f)] public float pitchMin = 0.85f;
    [Range(0.1f, 2f)] public float pitchMax = 1.15f;

    [Header("Sonidos de Armadura")]
    public AudioSource sourceArmadura;
    public AudioClip[] clipsArmadura;

    [Header("Combate")]
    public AudioSource sourceCombate;
    public AudioClip sonidoEspadazo; // Sonido de la espada al aire
    public AudioClip sonidoGolpeExitoso; // Sonido al impactar enemigo
    public AudioClip[] sonidosDolor; // Array para variaciones de quejidos


    
    public void PlayFootstep()
    {
        if (clipsPasos.Length == 0) return;

        // Elegimos un clip aleatorio
        int index = Random.Range(0, clipsPasos.Length);
        
        sourcePasos.pitch = Random.Range(pitchMin, pitchMax);
        sourcePasos.PlayOneShot(clipsPasos[index], volumenPasos);

        if (clipsArmadura.Length > 0)
        {
            int indexArm = Random.Range(0, clipsArmadura.Length);
            sourceArmadura.PlayOneShot(clipsArmadura[indexArm], volumenPasos * 0.7f);
        }
    }


    public void PlayEspadazo()
    {
        sourceCombate.PlayOneShot(sonidoEspadazo);
    }

    public void PlayGolpeExitoso()
    {
        sourceCombate.PlayOneShot(sonidoGolpeExitoso);
    }

    public void PlayDolor()
    {
        if (sonidosDolor.Length > 0)
        {
            int index = Random.Range(0, sonidosDolor.Length);
            sourceCombate.PlayOneShot(sonidosDolor[index]);
        }
    }



}