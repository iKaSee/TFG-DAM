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
    public float danio = 20f; // <--- ESTO FALTABA
    public float cooldownAtaque = 2f;
    public LayerMask capaJugador;


    [Header("Fase Berserker")]
    public float multiplicadorVelocidad = 1.5f; // 50% más rápido
    public float multiplicadorAtaque = 0.5f;    // Reduce el tiempo entre ataques (ataca más rápido)
    public bool enFase2 = false;
    public float multiplicadorDanio = 2f; // El daño se multiplicará por 2
    public GameObject ParticulasFuria; // Referencia a las partículas de furia
    public float velocidadAnimacionFase2 = 1.2f; // Velocidad de animación aumentada para la fase 2

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

        // --- NUEVA COMPROBACIÓN ---
        // Si el animador está reproduciendo la animación de ataque, NO hacemos nada más (se queda quieto)
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Atack_Bandit"))
        {
            // Opcional: Si quieres asegurar que no se mueva nada, puedes poner la velocidad a cero
            // rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

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

        if (jugador.position.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

    void Parar()
    {
        if (anim != null) anim.SetBool("IsWalking", false);
    }

    void IntentarAtacar()
    {
        Parar();
        if (Time.time >= tiempoSiguienteAtaque)
        {
            if (anim != null) anim.SetTrigger("Attack"); // Disparamos la animación
            tiempoSiguienteAtaque = Time.time + cooldownAtaque;

            // BORRAMOS la línea del Invoke(nameof(CausarDanio), 0.5f);
            // Ya no necesitamos que el código cuente el tiempo, lo hará la animación.
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

    
public void EntrarEnFaseBerserker(){
        if (enFase2) return;
        enFase2 = true;

        // 1. Más velocidad de movimiento
        velocidad *= multiplicadorVelocidad;
        // 2. Ataques más frecuentes
        cooldownAtaque *= multiplicadorAtaque;
        // 3. MÁS DAÑO (Modificamos la variable 'danio' que ya tienes)
        danio *= multiplicadorDanio;

        if (anim != null)
        {
            anim.speed = velocidadAnimacionFase2;
        }

        // Activar efectos visuales y música que ya teníamos
        if (ParticulasFuria != null) ParticulasFuria.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.6f);
        
        BossMusicController music = FindObjectOfType<BossMusicController>();
        if (music != null) music.CambiarAFase2();

        Debug.Log("¡BOSS ENFURECIDO: Más rápido, más daño y más agresivo!");
   
}

    public void ApagarFaseBerserker()
    {
        if (ParticulasFuria != null)
        {
            // Esto apaga el objeto de golpe
            ParticulasFuria.SetActive(false);
        }
    }
}