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

    private PlayerPotions playerPotions;
    private PlayerHealth playerHealth;

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

        if (playerPotions != null) playerPotions.RestablecerPociones();
        if (playerHealth != null) playerHealth.Heal(9999);

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
            playerPotions = other.GetComponent<PlayerPotions>();
            playerHealth = other.GetComponent<PlayerHealth>();
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