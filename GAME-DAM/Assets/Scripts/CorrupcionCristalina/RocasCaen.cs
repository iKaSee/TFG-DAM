using System.Collections;
using UnityEngine;

public class RocasCaen : MonoBehaviour
{
    [SerializeField] private Transform[] rocas;
    [SerializeField] private Transform[] posicionesFinales;
    [SerializeField] private AudioClip sonidoImpacto;
    [SerializeField] private float delayAntesDeCaer = 1.5f;
    [SerializeField] private float velocidadCaida = 3f;
    [SerializeField] private SecuenciaCristalina secuenciaCristalina;

    private Vector3[] posicionesInicio;

    void Awake()
    {
        posicionesInicio = new Vector3[rocas.Length];
        for (int i = 0; i < rocas.Length; i++)
        {
            posicionesInicio[i] = rocas[i].position;
        }
    }

    void Start()
    {
        StartCoroutine(SecuenciaRocas());
    }

    private IEnumerator SecuenciaRocas()
    {
        yield return new WaitForSeconds(delayAntesDeCaer);

        // La duración la marca la roca que tiene el recorrido más largo
        float distanciaMaxima = 0f;
        for (int i = 0; i < rocas.Length; i++)
        {
            float d = Vector3.Distance(posicionesInicio[i], posicionesFinales[i].position);
            if (d > distanciaMaxima) distanciaMaxima = d;
        }
        float duracion = distanciaMaxima / velocidadCaida;
        float tiempo = 0f;

        secuenciaCristalina.IniciarSecuencia();

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracion);
            // Ease-in cuadrático: arranca lento y acelera al impactar
            float eased = Mathf.Pow(progreso, 2f);

            for (int i = 0; i < rocas.Length; i++)
            {
                rocas[i].position = Vector3.Lerp(posicionesInicio[i], posicionesFinales[i].position, eased);
            }
            yield return null;
        }

        // Asegura el snap exacto al destino
        for (int i = 0; i < rocas.Length; i++)
        {
            rocas[i].position = posicionesFinales[i].position;
        }

        AudioSource.PlayClipAtPoint(sonidoImpacto, transform.position);
    }
}
