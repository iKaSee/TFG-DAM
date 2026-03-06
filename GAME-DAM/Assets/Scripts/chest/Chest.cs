using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private bool isOpened = false;

    [Header("Ajustes")]
    public int hitsToOpen = 1;
    private int currentHits = 0;

    [Header("Recompensa")]
    public GameObject itemPrefab; // Arrastra aquí una moneda o cristal

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Esta función la llamará Jotem al atacar
    public void TakeDamage(int damage)
    {
        if (isOpened) return;

        currentHits++;

        if (currentHits < hitsToOpen)
        {
            anim.SetTrigger("Hit"); // Animación de vibración
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

        // Desactivamos el collider para que no estorbe
        GetComponent<Collider2D>().enabled = false;

        // Soltamos la recompensa
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        Debug.Log("¡Cofre abierto!");
    }
}