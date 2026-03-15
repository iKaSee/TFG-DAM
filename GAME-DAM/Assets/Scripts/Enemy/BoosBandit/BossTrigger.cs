using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject objetoBarra;
    public BossGuardian elBoss;

    [Header("Lista de Objetos a Activar")]
    // Cambiamos GameObject por GameObject[] (esto crea una lista)
    public GameObject[] objetosBloqueo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objetoBarra != null) objetoBarra.SetActive(true);
            if (elBoss != null) elBoss.enabled = true;

            // Recorremos la lista y activamos cada uno
            foreach (GameObject obj in objetosBloqueo)
            {
                if (obj != null) obj.SetActive(true);
            }

            Destroy(gameObject);
        }
        // Dentro de OnTriggerEnter2D de BossTrigger.cs
        BossMusicController music = Object.FindAnyObjectByType<BossMusicController>();
        if (music != null)
        {
            music.EmpezarMusicaBoss();
        }
    }
}