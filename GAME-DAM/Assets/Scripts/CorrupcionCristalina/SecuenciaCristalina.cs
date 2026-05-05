using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;

public class SecuenciaCristalina : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcamJotem;
    [SerializeField] private CinemachineVirtualCamera vcamRocas;
    [SerializeField] private CinemachineVirtualCamera vcamEnemigo;
    [SerializeField] private GameObject canvasDialogo;
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private string[] lineasFueraDePlano;
    [SerializeField] private string[] lineasEnemigo;

    public void IniciarSecuencia()
    {
        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        PlayerController jugador = FindFirstObjectByType<PlayerController>();
        if (jugador != null) jugador.SetControlsEnabled(false);
        canvasUI.SetActive(false);

        // Cámara hacia las rocas mientras caen
        vcamRocas.Priority = 20;
        vcamJotem.Priority = 0;

        // El enemigo habla fuera de plano sobre las rocas
        canvasDialogo.SetActive(true);
        for (int i = 0; i < lineasFueraDePlano.Length; i++)
        {
            textoDialogo.text = lineasFueraDePlano[i];
            yield return new WaitForSeconds(3f);
        }
        canvasDialogo.SetActive(false);

        yield return new WaitForSeconds(3f);

        // Cámara revela al enemigo
        vcamEnemigo.Priority = 30;
        vcamRocas.Priority = 0;

        yield return new WaitForSeconds(1f);

        canvasDialogo.SetActive(true);
        for (int i = 0; i < lineasEnemigo.Length; i++)
        {
            textoDialogo.text = lineasEnemigo[i];
            yield return new WaitForSeconds(3f);
        }
        canvasDialogo.SetActive(false);

        // Devuelve la cámara al jugador y le restituye el control
        vcamJotem.Priority = 10;
        vcamEnemigo.Priority = 0;

        canvasUI.SetActive(true);
        if (jugador != null) jugador.SetControlsEnabled(true);
    }
}
