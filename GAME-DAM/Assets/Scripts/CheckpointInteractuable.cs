using UnityEngine;

public class CheckpointInteractuable : MonoBehaviour
{
    [Header("Configuracion")]
    public GameObject cartelAyuda; // El texto "Presiona G"
    public Animator animadorPersonaje; // Arrastra aquí el Animator del personaje

    private bool jugadorCerca = false;
    private bool yaSeGuardo = false; // Control para que solo sea una vez

    private void Update()
    {
        // Solo permite guardar si está cerca, presiona G y NO ha guardado antes
        if (jugadorCerca && Input.GetKeyDown(KeyCode.G) && !yaSeGuardo)
        {
            EjecutarGuardado();
        }
    }

    private void EjecutarGuardado()
    {
        yaSeGuardo = true; // Bloqueamos futuros guardados

        // Guardamos la posición en memoria
        PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
        PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
        PlayerPrefs.Save();

        Debug.Log("Partida guardada y bloqueada para este checkpoint.");

        // 1. Desactivamos el cartel de ayuda definitivamente
        if (cartelAyuda != null) cartelAyuda.SetActive(false);

        // 2. Ejecutamos tu animación
        if (animadorPersonaje != null)
        {
            animadorPersonaje.SetTrigger("Guardar");
            // Nota: Asegúrate de que el Trigger en el Animator se llame exactamente "Guardar"
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo mostramos el cartel si es el jugador y aún no ha guardado aquí
        if (other.CompareTag("Player") && !yaSeGuardo)
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