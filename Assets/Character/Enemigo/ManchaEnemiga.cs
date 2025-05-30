using UnityEngine;

public class ManchaEnemiga : MonoBehaviour
{
    private bool destruido = false;

    public void SerTocado(Transform jugador)
    {
        if (destruido) return;

        LiveSystem vidas = jugador.GetComponentInParent<LiveSystem>();
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

    public void Destruir()
    {
        destruido = true;
        Destroy(gameObject);
    }
}
