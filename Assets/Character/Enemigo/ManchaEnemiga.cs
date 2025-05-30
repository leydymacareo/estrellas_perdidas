using UnityEngine;

public class ManchaEnemiga : MonoBehaviour
{
    public void SerTocado(Transform jugador)
    {
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
}
