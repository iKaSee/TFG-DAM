using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    Transform cam; //Main Camera
    Vector3 camStartPos;
    float distanceX;
    float distanceY; // Añadimos distancia vertical para la caída

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    [Header("Control de Activación")]
    public bool estaActivo = false; // El interruptor para el bosque

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++) //find the farthest background
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++) //set the speed of bacground
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        // Si no está activo (estás en el túnel), el fondo se queda quieto
        if (!estaActivo)
        {
            // Opcional: Mantener el fondo centrado con la cámara pero sin mover textura
            transform.position = new Vector3(cam.position.x, cam.position.y, 9.92f);
            return;
        }

        // Calculamos la distancia recorrida desde que se activó o desde el inicio
        distanceX = cam.position.x - camStartPos.x;
        distanceY = cam.position.y - camStartPos.y;

        // El contenedor del fondo sigue a la cámara
        transform.position = new Vector3(cam.position.x, cam.position.y, 9.92f);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            // Aplicamos el movimiento de textura en X y en Y para la transición del bosque
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX, distanceY) * speed);
        }
    }

    // Función para activar el efecto desde un Trigger
    public void ActivarParallax()
    {
        estaActivo = true;
        // Reseteamos el punto de inicio para que el movimiento sea fluido al empezar
        camStartPos = cam.position;
    }
}