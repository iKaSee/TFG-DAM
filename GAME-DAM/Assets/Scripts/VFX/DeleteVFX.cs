using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    void Start()
    {
        // Se borra a los 0.4 segundos (lo que duran los 4 frames)
        Destroy(gameObject, 0.4f);
    }
}