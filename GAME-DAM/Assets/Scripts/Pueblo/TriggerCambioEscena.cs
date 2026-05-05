using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCambioEscena : MonoBehaviour
{
    [SerializeField] private string nombreEscena;

    private bool yaActivado;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaActivado || !other.CompareTag("Player")) return;

        yaActivado = true;

        PlayerPrefs.SetFloat("CheckpointX", 95.91f);
        PlayerPrefs.SetFloat("CheckpointY", 2f);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nombreEscena);
    }
}
