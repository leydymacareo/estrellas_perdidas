using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LiveSystem : MonoBehaviour
{
    [Header("Vidas")]
    public int maxLives = 3;
    private int currentLives;
    public Transform respawnPoint;
    public TextMeshProUGUI livesText;
    private float ultimoDaño = -999f;
    public float tiempoInvulnerabilidad = 1.0f;

    [Header("Pantalla de Game Over")]
    public GameObject gameOverPanel; // Asigna el panel desde el inspector

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        if (respawnPoint == null)
        {
            respawnPoint = this.transform;
        }
    }

    void Update()
    {
        UpdateLivesUI();
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"{currentLives}/{maxLives}";
        }
    }

    public void QuitarVida()
    {
        if (Time.time - ultimoDaño < tiempoInvulnerabilidad)
        {
            return; // Jugador aún está invulnerable
        }

        ultimoDaño = Time.time;
        currentLives--;
        Debug.Log("💥 Vidas restantes: " + currentLives);

        if (currentLives <= -1)
        {
            GameOver();
            return;
        }
        else
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = respawnPoint.position;
                controller.enabled = true;
            }
            else
            {
                transform.position = respawnPoint.position;
            }
        }

        UpdateLivesUI();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            QuitarVida(); // ✅ Ya maneja invulnerabilidad y respawn correctamente
        }
    }

    public void AgregarVida()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLivesUI();
            Debug.Log("Vida agregada. Vidas actuales: " + currentLives);
        }
    }

    private void GameOver()
    {
        Debug.Log("🛑 Game Over - sin vidas");

        Time.timeScale = 0f; // Pausar el juego

        // 🔓 Mostrar y desbloquear el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}

