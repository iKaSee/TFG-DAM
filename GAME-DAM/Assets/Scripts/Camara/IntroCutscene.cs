using UnityEngine;
using System.Collections;
using Cinemachine;

public class IntroCutscene : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera vcamJotem;
    public CinemachineVirtualCamera vcamIntro;

    [Header("Enemigos a Activar")]
    public Animator animMiniBoss;
    public Animator animArquero;

    [Header("Configuración")]
    public float tiempoEnfoque = 3f;
    private bool activada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activada)
        {
            activada = true;
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
            playerAnim.SetFloat("HorizontalSpeed", 0f);            // Si tienes otros (ej. IsRunning, IsJumping), ponlos también aquí
        }

        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return null; 

        if (controller != null) controller.enabled = false;

        vcamJotem.Priority = 10;
        vcamIntro.Priority = 20; 

        yield return new WaitForSeconds(1f);
        
        if (animMiniBoss != null) animMiniBoss.SetTrigger("Spawn"); 
        if (animArquero != null) animArquero.SetTrigger("Spawn");

        yield return new WaitForSeconds(tiempoEnfoque);

        vcamIntro.Priority = 5; 
        
        yield return new WaitForSeconds(1.5f);
        
        if (controller != null) controller.enabled = true;

        Destroy(gameObject);
    }
}