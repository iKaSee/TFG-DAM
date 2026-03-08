using UnityEngine;

public class ControladorZonaPaz : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.soloCaminar = true;

            collision.GetComponent<Rigidbody2D>().linearVelocity *= 0.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().soloCaminar = false;
            Debug.Log("Jotem recupera sus habilidades");
        }
    }
}