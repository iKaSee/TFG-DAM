using UnityEngine;

public class ViejoTed : MonoBehaviour
{
    [SerializeField] private string[] lineas;
    [SerializeField] private DialogoUI dialogoUI;
    [SerializeField] private PantallaHabilidades pantallaHabilidades;

    private bool jugadorCerca;
    private int lineaActual;
    private bool habilidadesDesbloqueadas = false;

    void Update()
    {
        if (!jugadorCerca || habilidadesDesbloqueadas || !Input.GetKeyDown(KeyCode.G)) return;

        if (lineaActual < lineas.Length)
        {
            MostrarLineaConIndicador(lineaActual);
            lineaActual++;
        }
        else
        {
            dialogoUI.Ocultar();
            habilidadesDesbloqueadas = true;
            DesbloquearHabilidades();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (!habilidadesDesbloqueadas)
            {
                MostrarLineaConIndicador(0);
                lineaActual = 1;
            }
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

    public void DesbloquearHabilidades()
    {
        Debug.Log("Habilidades desbloqueadas");
        pantallaHabilidades.MostrarPantalla();

        PlayerController pc = Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            pc.DesbloquearSprint();
            PlayerFury fury = pc.GetComponent<PlayerFury>();
            if (fury != null) fury.DesbloquearFuria();
        }
    }

    private void MostrarLineaConIndicador(int index)
    {
        string texto = lineas[index];
        if (index < lineas.Length - 1) texto += "...";
        dialogoUI.MostrarLinea(texto);
    }
}
