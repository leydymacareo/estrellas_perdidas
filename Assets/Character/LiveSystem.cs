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
        currentLives--;
        Debug.Log("Vidas restantes: " + currentLives);

        if (currentLives <= -1)
        {
            SceneManager.LoadScene("PlatformerGame");
        }
        else
        {
            transform.position = respawnPoint.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            currentLives--;
            Debug.Log("Vidas restantes: " + currentLives);

            if (currentLives <= -1)
            {
                SceneManager.LoadScene("PlatformerGame");
            }
            else
            {
                transform.position = respawnPoint.position;
            }
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

    
}

