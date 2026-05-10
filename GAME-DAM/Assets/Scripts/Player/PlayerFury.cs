using UnityEngine;
using System.Collections;

public class PlayerFury : MonoBehaviour
{
    [Header("Configuración")]
    public bool furyDesbloqueada = false;
    public float furyDuration = 15f;
    public float furyCooldown = 30f;

    [Header("Multiplicadores")]
    public float damageMultiplier = 1.5f;
    public float speedBonus = 0.5f;

    [Header("VFX")]
    public GameObject vfxRoarStart;
    public GameObject vfxRoarLoopFront;
    public GameObject vfxRoarLoopBack;
    [SerializeField] private Vector3 offsetVFX = new Vector3(0, 0.5f, 0);

    [Header("Tinte Rojo")]
    public Color tinteFuria = new Color(1f, 0.4f, 0.4f, 1f);

    [SerializeField] private IconoFuriaUI iconoFuriaUI;

    private bool isFuryActive = false;
    private bool enCooldown = false;
    private int damageOriginal;
    private float speedOriginal;

    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (furyDesbloqueada && Input.GetKeyDown(KeyCode.Q) && !isFuryActive && !enCooldown)
        {
            ActivarFuria();
        }
    }

    public void ActivarFuria()
    {
        StartCoroutine(RutinaFuria());
    }

    private IEnumerator RutinaFuria()
    {
        isFuryActive = true;
        if (iconoFuriaUI != null) iconoFuriaUI.IniciarCooldown(furyDuration + furyCooldown);

        anim.SetTrigger("roarStart");
        anim.SetBool("isRoaring", true);

        if (vfxRoarStart != null) Instantiate(vfxRoarStart, transform.position + offsetVFX, Quaternion.identity);

        GameObject loopFront = null;
        GameObject loopBack = null;

        damageOriginal = playerCombat.attackDamage;
        speedOriginal = playerController.MoveSpeed;

        playerCombat.attackDamage = (int)(damageOriginal * damageMultiplier);
        playerController.MoveSpeed = speedOriginal + speedBonus;

        yield return new WaitForSeconds(1.8f);
        if (vfxRoarLoopFront != null) loopFront = Instantiate(vfxRoarLoopFront, transform.position + offsetVFX, Quaternion.identity, transform);
        if (vfxRoarLoopBack != null) loopBack = Instantiate(vfxRoarLoopBack, transform.position + offsetVFX, Quaternion.identity, transform);

        yield return new WaitForSeconds(1.2f);
        anim.SetBool("isRoaring", false);

        float tiempo = 0f;
        while (tiempo < furyDuration)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / furyDuration;
            spriteRenderer.color = Color.Lerp(tinteFuria, Color.white, t);
            yield return null;
        }

        playerCombat.attackDamage = damageOriginal;
        playerController.MoveSpeed = speedOriginal;
        spriteRenderer.color = Color.white;

        if (loopFront != null) Destroy(loopFront);
        if (loopBack != null) Destroy(loopBack);

        isFuryActive = false;
        StartCoroutine(RutinaCooldown());
    }

    private IEnumerator RutinaCooldown()
    {
        enCooldown = true;
        yield return new WaitForSeconds(furyCooldown);
        enCooldown = false;
    }

    public void DesbloquearFuria()
    {
        furyDesbloqueada = true;
        if (iconoFuriaUI != null) iconoFuriaUI.gameObject.SetActive(true);
    }
}
