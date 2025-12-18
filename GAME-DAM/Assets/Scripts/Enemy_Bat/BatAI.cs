using UnityEngine;

public class BatAI : MonoBehaviour
{
    [Header("Deteccion")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    private bool awake = false;

    [Header("Movimiento")]
    public float speed = 3f;
    private Transform playerTransform;

    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Si el enemigo está recibiendo un golpe (Knockback), no hacemos nada más
        if (GetComponent<EnemyHealth>().IsInKnockback()) return;

        if (!awake) CheckForPlayer();
        else FollowPlayer();
    }

    void CheckForPlayer()
    {
        // Crea un circulo invisible para detectar si el Player entra en rango
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider != null)
        {
            playerTransform = playerCollider.transform;
            WakeUp();
        }
    }

    public void WakeUp()
    {
        awake = true;
        anim.SetBool("isAwake", true);
    }

    void FollowPlayer()
    {
        if (playerTransform == null) return;

        // Calculamos la direccion hacia el jugador
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Movemos el Rigidbody hacia el jugador
        rb.linearVelocity = direction * speed;

        // Girar el sprite segun la direccion
        if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }

    // Dibujar el rango de vision en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}