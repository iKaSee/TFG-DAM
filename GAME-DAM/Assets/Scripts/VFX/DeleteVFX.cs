using UnityEngine;

public class DeleteVFX : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo en segundos antes de que el VFX desaparezca.")]
    public float tiempoDeVida = 0.5f; 

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}