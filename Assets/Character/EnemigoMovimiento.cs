using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Vector3 objetivoActual;

    void Start()
    {
        // Asegurar que el enemigo empiece en puntoA
        transform.position = puntoA.position;
        objetivoActual = puntoB.position;
    }

    void Update()
    {
        // Mover al enemigo hacia el objetivo actual
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);

        // Verificar si llegó al objetivo (con tolerancia)
        if ((transform.position - objetivoActual).sqrMagnitude < 0.01f)
        {
            // Cambiar al otro punto como objetivo
            objetivoActual = (objetivoActual == puntoA.position) ? puntoB.position : puntoA.position;

            // Voltear el sprite horizontalmente
            /*Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;*/
        }
    }
}
