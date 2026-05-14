using UnityEngine;

public class LaserProyectil : MonoBehaviour
{
    public float velocidad = 15f;
    public int damage = 50;
    public float lifeTime = 5f;

    private bool haImpactado = false;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Setup(Vector2 direccion)
    {
        transform.right = direccion;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (haImpactado) return;

        transform.Translate(Vector3.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (haImpactado) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Impacto();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Impacto();
        }
    }

    void Impacto()
    {
        haImpactado = true;
        if (anim != null) anim.SetTrigger("explotar");
        Destroy(gameObject, 0.5f);
    }
}
