using UnityEngine;
using System.Collections;
using Cinemachine;

public class IntroCutscene : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera vcamJotem;
    public CinemachineVirtualCamera vcamIntro;

    [Header("Enemigos a Activar")]
    public Animator[] enemigosAActivar; 

    [Header("Muros de Niebla")]
    public GameObject[] murosNiebla; 

    // --- REFERENCIA AL CANVAS DE LA VIDA ---
    [Header("UI del Boss")]
    public GameObject canvasVidaBoss;
    // ----------------------------------------------

    [Header("Música del Boss")] 
    public AudioSource musicaBoss; 

    [Header("Configuración")]
    public float tiempoEnfoque = 3f;
    private bool activada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activada)
        {
            activada = true;

            // --- REPRODUCIR MÚSICA AQUÍ ---
            if (musicaBoss != null)
            {
                musicaBoss.Play();
            }
            // ------------------------------

            StartCoroutine(SecuenciaIntro(other.gameObject));
        }
    }

    IEnumerator SecuenciaIntro(GameObject jugador)
    {
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        Animator playerAnim = jugador.GetComponent<Animator>();
        PlayerController controller = jugador.GetComponent<PlayerController>();

        if (playerAnim != null)
        {
            playerAnim.SetFloat("HorizontalSpeed", 0f); 
        }

        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return null; 

        if (controller != null) controller.enCinematica = true;

        foreach (GameObject muro in murosNiebla)
        {
            if (muro != null) muro.SetActive(true);
        }

        vcamJotem.Priority = 10;
        vcamIntro.Priority = 20; 

        yield return new WaitForSeconds(1f);
        
        foreach (Animator enemigo in enemigosAActivar)
        {
            if (enemigo != null) enemigo.SetTrigger("Spawn");
        }

        yield return new WaitForSeconds(tiempoEnfoque);

        vcamIntro.Priority = 5; 
        
        yield return new WaitForSeconds(1.5f);
        
        if (controller != null) controller.enCinematica = false;

        // --- ENCENDER LA BARRA DE VIDA AL FINAL ---
        if (canvasVidaBoss != null) canvasVidaBoss.SetActive(true);
        // -------------------------------------------------

        // --- CAMBIO AQUÍ: Destruimos solo el Collider para que el AudioSource no muera ---
        Destroy(GetComponent<Collider2D>());
    }
}