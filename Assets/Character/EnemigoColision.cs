using UnityEngine;

public class EnemigoColision : MonoBehaviour
{
    public float umbralDesdeArriba = 0.4f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 puntoContacto = collision.contacts[0].point;
            float alturaJugador = collision.transform.position.y;
            float alturaEnemigo = transform.position.y;

            if (alturaJugador > alturaEnemigo + umbralDesdeArriba)
            {
                // El jugador vino desde arriba: destruir enemigo
                Destroy(gameObject);
            }
            else
            {
                // El jugador fue tocado de lado: restar vida
                LiveSystem sistemaVidas = collision.gameObject.GetComponent<LiveSystem>();
                if (sistemaVidas != null)
                {
                    sistemaVidas.QuitarVida();
                }
            }
        }
    }
}