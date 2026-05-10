using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [Header("Configuración")]
    public bool bloqueoDesbloqueado = false;
    public float reduccionDaño = 0.5f;

    private bool isBlocking = false;
    private Animator anim;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private RigidbodyConstraints2D originalConstraints;

    public bool EstaBlockeando => isBlocking;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) originalConstraints = rb.constraints;

        if (PlayerPrefs.GetInt("BloqueoDesbloqueado", 0) == 1) bloqueoDesbloqueado = true;
    }

    void Update()
    {
        if (bloqueoDesbloqueado && Input.GetMouseButtonDown(1) && !isBlocking)
        {
            isBlocking = true;
            anim.SetBool("isBlocking", true);
            if (playerController != null) playerController.SetControlsEnabled(false);
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                rb.constraints = originalConstraints | RigidbodyConstraints2D.FreezePositionX;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            isBlocking = false;
            anim.SetBool("isBlocking", false);
            if (playerController != null) playerController.SetControlsEnabled(true);
            if (rb != null) rb.constraints = originalConstraints;
        }
    }

    void FixedUpdate()
    {
        if (isBlocking && rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    public bool RecibirGolpeBloqueado()
    {
        anim.SetTrigger("blockHit");
        return isBlocking;
    }

    public void DesbloquearBloqueo()
    {
        bloqueoDesbloqueado = true;
        PlayerPrefs.SetInt("BloqueoDesbloqueado", 1);
        PlayerPrefs.Save();
    }

    public float GetReduccionDaño()
    {
        return reduccionDaño;
    }
}
