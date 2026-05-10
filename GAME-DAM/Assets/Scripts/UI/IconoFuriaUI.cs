using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconoFuriaUI : MonoBehaviour
{
    [SerializeField] private Image iconoImage;
    [SerializeField] private Sprite spriteEstatico;
    [SerializeField] private Color colorCooldown = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Sprite[] spritesAnimacion;
    [SerializeField] private float fps = 12f;

    private bool enCooldown = false;

    void Awake()
    {
        StartCoroutine(AnimarIcono());
    }

    private IEnumerator AnimarIcono()
    {
        int index = 0;
        while (true)
        {
            if (!enCooldown && spritesAnimacion != null && spritesAnimacion.Length > 0 && iconoImage != null)
            {
                iconoImage.sprite = spritesAnimacion[index];
                index = (index + 1) % spritesAnimacion.Length;
            }
            yield return new WaitForSeconds(1f / fps);
        }
    }

    public void IniciarCooldown(float duracion)
    {
        enCooldown = true;
        if (iconoImage != null)
        {
            iconoImage.sprite = spriteEstatico;
            iconoImage.color = colorCooldown;
        }
        StartCoroutine(RutinaCooldown(duracion));
    }

    private IEnumerator RutinaCooldown(float duracion)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;
            if (iconoImage != null) iconoImage.color = Color.Lerp(colorCooldown, Color.white, t);
            yield return null;
        }

        enCooldown = false;
        if (iconoImage != null) iconoImage.color = Color.white;
    }
}
