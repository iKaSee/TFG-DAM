using UnityEngine;

public class Beggar : MonoBehaviour
{
    [SerializeField] private string[] lineas;
    [SerializeField] private DialogoUI dialogoUI;
    [SerializeField] private PantallaHabilidades pantallaPocion;

    private bool jugadorCerca;
    private int lineaActual;
    private bool pocionesDesbloqueadas = false;

    void Update()
    {
        if (!jugadorCerca || pocionesDesbloqueadas || !Input.GetKeyDown(KeyCode.G)) return;

        if (lineaActual < lineas.Length)
        {
            MostrarLineaConIndicador(lineaActual);
            lineaActual++;
        }
        else
        {
            dialogoUI.Ocultar();
            pocionesDesbloqueadas = true;
            DesbloquearPociones();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !pocionesDesbloqueadas)
        {
            jugadorCerca = true;
            MostrarLineaConIndicador(0);
            lineaActual = 1;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            dialogoUI.Ocultar();
            lineaActual = 0;
        }
    }

    private void DesbloquearPociones()
    {
        PlayerPotions pociones = Object.FindFirstObjectByType<PlayerPotions>();
        if (pociones != null) pociones.DesbloquearPociones(1);
        pantallaPocion.MostrarPantalla();
    }

    private void MostrarLineaConIndicador(int index)
    {
        string texto = lineas[index];
        if (index < lineas.Length - 1) texto += "...";
        dialogoUI.MostrarLinea(texto);
    }
}
