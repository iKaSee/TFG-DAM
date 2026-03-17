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

    [Header("Fase 3 (Poder Divino)")]
    public GameObject pilarLuzPrefab;      
    public float tiempoEntrePilares = 2f;  
    public bool enFase3 = false;


    private float tiempoSiguienteAtaque;
    private bool isAttacking = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        miSalud = GetComponent<BoosHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (miSalud.estaMuerto || jugador == null) 
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetBool("IsWalking", false);
            return;
        }

        if (jugador.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return; 
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= radioDeteccion)
        {
            if (distanciaAlJugador <= distanciaAtaque)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
                if (anim != null) anim.SetBool("IsWalking", false);

                if (Time.time >= tiempoSiguienteAtaque)
                {
                    isAttacking = true; // BLOQUEAMOS EL MOVIMIENTO
                    if (anim != null) anim.SetTrigger("Attack");
                    tiempoSiguienteAtaque = Time.time + cooldownAtaque;
                }
            }
            else
            {
                float dirX = jugador.position.x > transform.position.x ? 1 : -1;
                rb.linearVelocity = new Vector2(dirX * velocidad, rb.linearVelocity.y);
                
                if (anim != null) anim.SetBool("IsWalking", true); 
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetBool("IsWalking", false); 
        }
    }

    public void FinalizarAtaque()
    {
        isAttacking = false;
    }

    void Perseguir()
    {
        if (anim != null) anim.SetBool("IsWalking", true);
        Vector2 objetivo = new Vector2(jugador.position.x, rb.position.y);
        transform.position = Vector2.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        MirarAlJugador();
    }

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
                Debug.Log("�Impacto sincronizado con la animaci�n!");
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

        Debug.Log("�BOSS ENFURECIDO: M�s r�pido, m�s da�o y m�s agresivo!");
    }

    public void ApagarFaseBerserker()
    {
        if (ParticulasFuria != null)
        {
            ParticulasFuria.SetActive(false);
        }
    }

    public void EntrarEnFase3()
    {
        if (enFase3) return; 
        enFase3 = true;
        
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.8f, 1f); 

        StartCoroutine(RutinaInvocarPilares());
    }

    System.Collections.IEnumerator RutinaInvocarPilares()
    {
        while (enFase3 && !miSalud.estaMuerto)
        {
            if (pilarLuzPrefab != null && jugador != null)
            {
                Vector2 posicionRayo = new Vector2(jugador.position.x, jugador.position.y - 1f); 
                Instantiate(pilarLuzPrefab, posicionRayo, Quaternion.identity);
            }

            yield return new WaitForSeconds(tiempoEntrePilares);
        }
    }


}