using UnityEngine;

public class BossGuardian : MonoBehaviour
{
    [Header("Referencias")]
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator anim;
    private BoosHealth miSalud;

    [Header("IA Movimiento")]
    public float velocidad = 2.5f;
    public float radioDeteccion = 12f;
    public float distanciaAtaque = 2.2f;

    [Header("IA Combate")]
    public Transform puntoGolpe;
    public float radioGolpe = 1.5f;
    public float danio = 20f;
    public float cooldownAtaque = 2f;
    public LayerMask capaJugador;

    [Header("Fase Berserker")]
    public float multiplicadorVelocidad = 1.5f;
    public float multiplicadorAtaque = 0.5f;
    public bool enFase2 = false;
    public float multiplicadorDanio = 2f;
    public GameObject ParticulasFuria;
    public float velocidadAnimacionFase2 = 1.2f;

    private float tiempoSiguienteAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        miSalud = GetComponent<BoosHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (jugador == null || miSalud == null || miSalud.estaMuerto) return;

        // --- LÓGICA DE SEGUIMIENTO DURANTE ATAQUE ---
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool estaAtacando = stateInfo.IsName("Atack_Bandit");

        if (estaAtacando)
        {
            // Si la animación está en su fase inicial (menos del 20% de progreso),
            // el Boss aún puede girarse para seguir al jugador.
            if (stateInfo.normalizedTime < 0.5f)
            {
                MirarAlJugador();
            }

            // Mantenemos al Boss quieto mientras dura el ataque
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // --- LÓGICA DE COMPORTAMIENTO NORMAL ---
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia < radioDeteccion && distancia > distanciaAtaque)
        {
            Perseguir();
        }
        else if (distancia <= distanciaAtaque)
        {
            IntentarAtacar();
        }
        else
        {
            Parar();
        }
    }

    void Perseguir()
    {
        if (anim != null) anim.SetBool("IsWalking", true);
        Vector2 objetivo = new Vector2(jugador.position.x, rb.position.y);
        transform.position = Vector2.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        // Usamos la nueva función para girar
        MirarAlJugador();
    }

    // Nueva función extraída para controlar el giro (Flip)
    private void MirarAlJugador()
    {
        if (jugador == null) return;

        if (jugador.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Parar()
    {
        if (anim != null) anim.SetBool("IsWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void IntentarAtacar()
    {
        Parar();
        if (Time.time >= tiempoSiguienteAtaque)
        {
            if (anim != null) anim.SetTrigger("Attack");
            tiempoSiguienteAtaque = Time.time + cooldownAtaque;
        }
    }

    public void EventoDeGolpe()
    {
        if (puntoGolpe == null) return;

        Collider2D hit = Physics2D.OverlapCircle(puntoGolpe.position, radioGolpe, capaJugador);

        if (hit != null)
        {
            PlayerHealth saludJugador = hit.GetComponent<PlayerHealth>();
            if (saludJugador != null)
            {
                saludJugador.TakeDamage((int)danio);
                Debug.Log("¡Impacto sincronizado con la animación!");
            }
        }
    }

    public void EntrarEnFaseBerserker()
    {
        if (enFase2) return;
        enFase2 = true;

        velocidad *= multiplicadorVelocidad;
        cooldownAtaque *= multiplicadorAtaque;
        danio *= multiplicadorDanio;

        if (anim != null)
        {
            anim.speed = velocidadAnimacionFase2;
        }

        if (ParticulasFuria != null) ParticulasFuria.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.6f);

        BossMusicController music = Object.FindFirstObjectByType<BossMusicController>();
        if (music != null) music.CambiarAFase2();

        Debug.Log("¡BOSS ENFURECIDO: Más rápido, más daño y más agresivo!");
    }

    public void ApagarFaseBerserker()
    {
        if (ParticulasFuria != null)
        {
            ParticulasFuria.SetActive(false);
        }
    }
}