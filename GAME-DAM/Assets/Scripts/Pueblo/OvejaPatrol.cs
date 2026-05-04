using System.Collections;
using UnityEngine;

public class OvejaPatrol : MonoBehaviour
{
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private AudioClip sonidoOveja;

    private Animator anim;
    private Transform objetivo;
    private bool esperando;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        objetivo = puntoB;
        anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (esperando) return;

        transform.position = Vector2.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);

        float direccion = objetivo.position.x - transform.position.x;
        if (Mathf.Abs(direccion) > 0.01f)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x) * (direccion < 0f ? -1f : 1f);
            transform.localScale = escala;
        }

        if (Vector2.Distance(transform.position, objetivo.position) < 0.05f)
        {
            StartCoroutine(EsperarEnPunto());
        }
    }

    private IEnumerator EsperarEnPunto()
    {
        esperando = true;
        anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(2f);

        AudioSource.PlayClipAtPoint(sonidoOveja, transform.position);

        objetivo = objetivo == puntoA ? puntoB : puntoA;
        anim.SetBool("isWalking", true);
        esperando = false;
    }
}
