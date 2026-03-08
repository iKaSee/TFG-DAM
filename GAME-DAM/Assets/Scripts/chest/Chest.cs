using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private bool isOpened = false;

    [Header("Ajustes")]
    public int hitsToOpen = 1;
    private int currentHits = 0;

    [Header("Recompensa")]
    public GameObject itemPrefab; // Tu prefab de moneda
    public int cantidadMonedas = 5; // Cuántas monedas soltará
    public float fuerzaExplosion = 5f; // Fuerza con la que saltan

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isOpened) return;

        currentHits++;

        if (currentHits < hitsToOpen)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        anim.SetTrigger("Open");

        // Desactivamos el collider del cofre para que las monedas no choquen con él al salir
        GetComponent<Collider2D>().enabled = false;

        // Soltamos la lluvia de monedas
        if (itemPrefab != null)
        {
            for (int i = 0; i < cantidadMonedas; i++)
            {
                // Creamos la moneda un poco por encima del cofre
                GameObject moneda = Instantiate(itemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

                // Le damos un impulso físico aleatorio
                Rigidbody2D rbMoneda = moneda.GetComponent<Rigidbody2D>();
                if (rbMoneda != null)
                {
                    // Salto en abanico: un poco a la izquierda/derecha y siempre hacia arriba
                    Vector2 direccionSalto = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 1.5f));
                    rbMoneda.AddForce(direccionSalto * fuerzaExplosion, ForceMode2D.Impulse);
                }
            }
        }

        Debug.Log("¡Cofre abierto con botín!");
    }
}