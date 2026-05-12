using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerPotions : MonoBehaviour
{
    [Header("Configuración")]
    public bool pocionesDesbloqueadas = false;
    public int maxPociones = 0;
    public int pocionesActuales = 0;
    public int curacionPorPocion = 50;

    [Header("UI")]
    [SerializeField] private GameObject iconoPociones;
    [SerializeField] private TextMeshProUGUI textoCantidad;

    private bool bebiendo = false;
    private Animator anim;
    private PlayerHealth playerHealth;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (PlayerPrefs.GetInt("PocionesDesbloqueadas", 0) == 1)
        {
            pocionesDesbloqueadas = true;
            maxPociones = PlayerPrefs.GetInt("MaxPociones", 1);
            pocionesActuales = PlayerPrefs.GetInt("PocionesActuales", 0);
            if (iconoPociones != null) iconoPociones.SetActive(true);
            ActualizarUI();
        }
    }

    void Update()
    {
        if (pocionesDesbloqueadas && Input.GetKeyDown(KeyCode.E) && pocionesActuales > 0 && !bebiendo)
        {
            StartCoroutine(UsarPocion());
        }
    }

    private IEnumerator UsarPocion()
    {
        bebiendo = true;
        anim.SetTrigger("drinkPotion");

        yield return new WaitForSeconds(1.8f);

        playerHealth.Heal(curacionPorPocion);
        pocionesActuales--;
        PlayerPrefs.SetInt("PocionesActuales", pocionesActuales);
        PlayerPrefs.Save();
        ActualizarUI();

        bebiendo = false;
    }

    private void ActualizarUI()
{
    if (textoCantidad != null) textoCantidad.text = pocionesActuales.ToString();
    Debug.Log("Pociones actuales: " + pocionesActuales + " | textoCantidad null: " + (textoCantidad == null));
}

    public void DesbloquearPociones(int cantidad)
    {
        pocionesDesbloqueadas = true;
        maxPociones = cantidad;
        pocionesActuales = cantidad;
        if (iconoPociones != null) iconoPociones.SetActive(true);
        PlayerPrefs.SetInt("PocionesDesbloqueadas", 1);
        PlayerPrefs.SetInt("MaxPociones", maxPociones);
        PlayerPrefs.Save();
        ActualizarUI();
    }

    public void AñadirPociones(int cantidad)
    {
        maxPociones += cantidad;
        pocionesActuales += cantidad;
        PlayerPrefs.SetInt("MaxPociones", maxPociones);
        PlayerPrefs.Save();
        ActualizarUI();
    }

    public void RestablecerPociones()
    {
        pocionesActuales = maxPociones;
        PlayerPrefs.SetInt("PocionesActuales", pocionesActuales);
        PlayerPrefs.Save();
        ActualizarUI();
    }
}
