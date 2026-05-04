using UnityEngine;
using System.Collections;


public class Hoguera : MonoBehaviour
{
    [Header("Configuración")]
    public Transform puntoAparicion; 
    private bool jugadorCerca = false;
    private bool yaActivada = false;

    [Header("Efectos")]
    public GameObject vfxActivacion;

    [Header("UI")]
    [SerializeField] private GameObject promptTecla;

    void Update()
    {
        // Si el jugador está cerca y pulsa G
        if (jugadorCerca && Input.GetKeyDown(KeyCode.G))
        {
            DescansarEnHoguera();
        }
    }

    void DescansarEnHoguera()
    {
        PlayerPrefs.SetFloat("CheckpointX", puntoAparicion.position.x);
        PlayerPrefs.SetFloat("CheckpointY", puntoAparicion.position.y);
        PlayerPrefs.SetInt("HogueraActivada", 1); 
        PlayerPrefs.Save();

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.SecuenciaHoguera(false);
        }

        if (!yaActivada)
        {
            yaActivada = true;
            if (vfxActivacion != null) Instantiate(vfxActivacion, transform.position, Quaternion.identity);
            Debug.Log("Hoguera encendida y Checkpoint guardado.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (promptTecla != null) promptTecla.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (promptTecla != null) promptTecla.SetActive(false);
        }
    }
}