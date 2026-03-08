using UnityEngine;

public class ItemMoneda : MonoBehaviour
{
    public int valorMoneda = 10;
    private bool puedeRecogerse = false;
    private float tiempoEspera = 0.5f; // Medio segundo de vuelo antes de poder cogerla

    void Start()
    {
        // Al aparecer, esperamos un poco antes de activar la recogida
        Invoke("ActivarRecogida", tiempoEspera);
    }

    void ActivarRecogida() => puedeRecogerse = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo la recogemos si ha pasado el tiempo y es el Player
        if (puedeRecogerse && collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.SumarOro(valorMoneda);
            }

            // Aquí podrías instanciar un efecto de brillo
            Destroy(gameObject);
        }
    }
}