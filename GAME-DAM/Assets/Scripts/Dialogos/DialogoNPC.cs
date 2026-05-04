using UnityEngine;

public class DialogoNPC : MonoBehaviour
{
    [SerializeField] private string[] lineas;
    [SerializeField] private DialogoUI dialogoUI;

    private bool jugadorCerca;
    private bool dialogoActivo;
    private int lineaActual;

    void Update()
    {
        if (!jugadorCerca || !Input.GetKeyDown(KeyCode.G)) return;

        if (lineaActual < lineas.Length)
        {
            MostrarLineaConIndicador(lineaActual);
            dialogoActivo = true;
            lineaActual++;
        }
        else
        {
            dialogoUI.Ocultar();
            dialogoActivo = false;
            lineaActual = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            // Muestra la primera línea automáticamente al acercarse
            MostrarLineaConIndicador(0);
            dialogoActivo = true;
            lineaActual = 1;
        }
        Debug.Log("Trigger detectado: " + other.tag);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            dialogoUI.Ocultar();
            dialogoActivo = false;
            lineaActual = 0;
        }
    }

    // Añade "..." cuando aún quedan líneas por mostrar
    private void MostrarLineaConIndicador(int index)
    {
        string texto = lineas[index];
        if (index < lineas.Length - 1) texto += "...";
        dialogoUI.MostrarLinea(texto);
    }
}
