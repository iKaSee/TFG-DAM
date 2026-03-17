using UnityEngine;

public class EnemyArcher : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject ArrowPrefab;
    public Transform shootPoint; 
    private Animator anim;

    [Header("Ajustes de Ataque")]
    public float range = 5f;
    public float timeBetweenShots = 2f;
    private float nextShotTime;

    [Header("Estado")]
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        // Si no asignas al jugador en el inspector, lo busca por el Tag
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= range)
        {
            anim.SetBool("isPlayerInRange", true);
            LookAtPlayer();
            
            ApuntarArco();

            if (Time.time >= nextShotTime)
            {
                Shoot();
                nextShotTime = Time.time + timeBetweenShots;
            }
        }
        else
        {
            anim.SetBool("isPlayerInRange", false);
        }
    }


    void ApuntarArco()
    {
        if (shootPoint == null) return;

        Vector3 direccion = player.position - shootPoint.position;
        
        float anguloZ = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        shootPoint.rotation = Quaternion.Euler(0, 0, anguloZ);
    }


    void LookAtPlayer()
    {
        // Giramos al arquero hacia la posición de Jotem
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    void Shoot()
    {
        anim.SetTrigger("Shoot");
    }

    // ESTA FUNCIÓN SE LLAMA DESDE EL ANIMATION Event
    public void LanzarFlecha()
    {
        if (isDead) return;
        
        GameObject arrow = Instantiate(ArrowPrefab, shootPoint.position, shootPoint.rotation);
        
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null) {
            arrowScript.Setup(1f); // Pasamos 1 porque la flecha ya va rotada
        }
    }

    public void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        // Desactivamos el script y colisionadores si es necesario
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 3f);
    }
}