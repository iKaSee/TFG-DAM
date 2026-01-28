using UnityEngine;

public class CofreVida : MonoBehaviour
{
    [Header("Ajustes de Curación")]
    public int puntosCuracion = 40;
    public GameObject cartelAyuda; // El texto de "Presiona G"

    private bool jugadorCerca = false;
    private bool yaAbierto = false;

    [Header("Referencias")]
    public Animator animCofre;

    void Update()
    {
        // Si el jugador está en el rango, presiona G y el cofre está cerrado
        if (jugadorCerca && Input.GetKeyDown(KeyCode.G) && !yaAbierto)
        {
            AbrirCofre();
        }
    }

    void AbrirCofre()
    {
        yaAbierto = true;

        // 1. Animación del cofre (Trigger "Abrir")
        if (animCofre != null)
        {
            animCofre.SetTrigger("Abrir");
        }

        // 2. Curar al jugador
        PlayerHealth health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(puntosCuracion);
        }

        // 3. Quitar el cartel de ayuda para siempre
        if (cartelAyuda != null)
        {
            cartelAyuda.SetActive(false);
        }

        Debug.Log("Cofre abierto con la G.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaAbierto)
        {
            jugadorCerca = true;
            if (cartelAyuda != null) cartelAyuda.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (cartelAyuda != null) cartelAyuda.SetActive(false);
        }
    }
}