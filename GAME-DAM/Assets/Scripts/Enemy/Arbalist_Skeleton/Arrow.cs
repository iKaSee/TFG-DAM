using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 15;
    public float lifeTime = 5f; // Para que no vuelen infinitamente si fallan
    
    private float direction;
    private bool hitSomething = false;

    public void Setup(float dir)
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (hitSomething) return;
        
        // Movimiento constante
    transform.Translate(Vector3.right * speed * Time.deltaTime);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitSomething) return;

        // 1. ¿Golpeó a Jotem?
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Impacto();
        }
        
        // 2. ¿Golpeó el suelo o una pared?
        // Comprueba si la capa del objeto es la de "Ground" o "Plataformas"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Impacto();
        }
    }

    void Impacto()
    {
        hitSomething = true;
        // Aquí podrías instanciar un efecto de chispas o madera rota
        Destroy(gameObject); 
    }
}