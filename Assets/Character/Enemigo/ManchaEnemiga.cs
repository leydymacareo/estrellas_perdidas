using UnityEngine;

public class ManchaEnemiga : MonoBehaviour
{
    private bool derrotado = false;

    public void SerTocado(Transform jugador)
    {
        if (derrotado)
        {
            Debug.Log("🛡️ Mancha ya derrotada. No se quita vida.");
            return;
        }

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

    public void MarcarComoDerrotado()
    {
        derrotado = true;
    }
}
