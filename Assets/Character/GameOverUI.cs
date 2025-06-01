using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; // Quitar pausa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reiniciar escena
    }
}
