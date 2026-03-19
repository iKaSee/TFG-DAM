using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private bool isOpened = false;

    [Header("Ajustes")]
    public int hitsToOpen = 1;
    private int currentHits = 0;

    [Header("Recompensa")]
    public GameObject itemPrefab; 
    public int cantidadMonedas = 5; 
    public float fuerzaExplosion = 5f; 

    [Header("Ajustes de Emboscada (Opcional)")]
    public bool esCofreTrampa = false; 
    public int golpeParaEmboscada = 3;
    public GameObject enemigoEmboscadaPrefab; 
    public Transform puntoAparicion; 
    private bool emboscadaActivada = false;
    // 

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isOpened) return;

        currentHits++;

        if (esCofreTrampa && !emboscadaActivada && currentHits == golpeParaEmboscada)
        {
            ActivarEmboscada();
        }

        if (currentHits < hitsToOpen)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            OpenChest();
        }
    }

    void ActivarEmboscada()
    {
        emboscadaActivada = true;
        
        if (enemigoEmboscadaPrefab != null)
        {
            Vector3 posicionSpawn = puntoAparicion != null ? puntoAparicion.position : transform.position + new Vector3(1.5f, 0, 0);
            Instantiate(enemigoEmboscadaPrefab, posicionSpawn, Quaternion.identity);
            
        }
    }
    // --------------------------------------------

    void OpenChest()
    {
        isOpened = true;
        anim.SetTrigger("Open");

        GetComponent<Collider2D>().enabled = false;

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