using UnityEngine;

public class ManchaEnemiga : MonoBehaviour
{
    private bool destruido = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (destruido) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            LiveSystem vidas = collision.gameObject.GetComponentInParent<LiveSystem>();
            if (vidas != null)
            {
                Debug.Log("💥 Jugador tocó la mancha. Vida menos.");
                vidas.QuitarVida();
            }
            else
            {
                Debug.LogWarning("❌ No se encontró LiveSystem en el jugador o su padre.");
            }
        }
    }

    public void Destruir()
    {
        destruido = true;
        Destroy(gameObject);
    }
}
