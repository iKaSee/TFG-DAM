using UnityEngine;

public class PosicionarJugador : MonoBehaviour
{
    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            jugador.transform.position = transform.position;

            Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            Debug.Log("Jotem posicionado correctamente en la escena del Boss.");
        }
        else
        {
            Debug.LogError("No se ha encontrado a Jotem. ¿Seguro que tiene el Tag 'Player' puesto?");
        }
    }
}