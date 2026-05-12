using UnityEngine;

public class HerreroJoe : MonoBehaviour
{
    [SerializeField] private string[] lineas;
    [SerializeField] private DialogoUI dialogoUI;
    [SerializeField] private PantallaHabilidades pantallaHerreroJoe;

    private bool jugadorCerca;
    private int lineaActual;
    private bool yaHablado = false;

    void Update()
    {
        if (!jugadorCerca || yaHablado || !Input.GetKeyDown(KeyCode.G)) return;

        if (lineaActual < lineas.Length)
        {
            MostrarLineaConIndicador(lineaActual);
            lineaActual++;
        }
        else
        {
            dialogoUI.Ocultar();
            yaHablado = true;
            DesbloquearMejoras();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaHablado)
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

    public void DesbloquearMejoras()
    {
        PlayerPotions playerPotions = Object.FindFirstObjectByType<PlayerPotions>();
        if (playerPotions != null) playerPotions.AñadirPociones(2);

        PlayerCombat playerCombat = Object.FindFirstObjectByType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.attackDamage += 10;
            PlayerPrefs.SetInt("AttackDamage", playerCombat.attackDamage);
            PlayerPrefs.Save();
        }

        PlayerBlock playerBlock = Object.FindFirstObjectByType<PlayerBlock>();
        if (playerBlock != null) playerBlock.DesbloquearBloqueo();

        Debug.Log("DesbloquearMejoras llamado");
        pantallaHerreroJoe.MostrarPantalla();
    }

    private void MostrarLineaConIndicador(int index)
    {
        string texto = lineas[index];
        if (index < lineas.Length - 1) texto += "...";
        dialogoUI.MostrarLinea(texto);
    }
}
