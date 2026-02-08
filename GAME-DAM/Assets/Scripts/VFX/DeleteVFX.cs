using UnityEngine;
public class DeleteVFX : MonoBehaviour
{
    void Start()
    {
        // Se destruye a los 0.5 segundos (ajusta al tiempo de tu animación)
        Destroy(gameObject, 0.5f);
    }
}