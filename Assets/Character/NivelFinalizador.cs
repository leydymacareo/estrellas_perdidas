using UnityEngine;

public class NivelFinalizador : MonoBehaviour
{
    public GameObject victoryPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f; // Pausar el juego
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }

            Debug.Log("🎉 ¡Nivel completado!");
        }
    }
}
