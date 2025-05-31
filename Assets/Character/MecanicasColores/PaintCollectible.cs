using UnityEngine;

[RequireComponent(typeof(Collider))] // Asegura que el GameObject tiene un Collider para detectar colisiones
[RequireComponent(typeof(Rigidbody))] // Asegura que el GameObject tiene un Rigidbody para que OnTriggerEnter funcione
public class PaintCollectible : MonoBehaviour
{
    [Tooltip("El color que se desbloqueará cuando se recolecte esta pintura. ¡Debe coincidir con un color definido en ColorManager con valores RGB exactos (0 o 255) y Alfa en 255!")]
    public Color unlocksColor = Color.red; // Color por defecto para esta pintura (ej. Rojo)

    void Awake()
    {
        // Configura el Collider para que sea un Trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("PaintCollectible: No se encontró un Collider en este GameObject (" + name + "). Es necesario para la recolección.", this);
            enabled = false;
        }

        // Configura el Rigidbody para que no afecte la física del mundo
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // Hace que el Rigidbody sea controlado por el script, no por la física
        }
        else
        {
            Debug.LogError("PaintCollectible: No se encontró un Rigidbody en este GameObject (" + name + "). Es necesario para que los triggers funcionen.", this);
            enabled = false;
        }
    }

    // Detecta cuando otro collider (esperamos que sea el jugador) entra en este trigger
    private void OnTriggerEnter(Collider other)
    {
        // Compara el tag del otro GameObject para asegurarse de que sea el jugador
        if (other.CompareTag("Player")) // ¡Asegúrate de que tu personaje tenga el Tag "Player"!
        {
            Debug.Log("Pintura " + unlocksColor.ToString() + " recolectada por el jugador.");

            // Llama al ColorManager para desbloquear este color
            if (ColorManager.Instance != null)
            {
                ColorManager.Instance.UnlockColor(unlocksColor);
            }
            else
            {
                Debug.LogError("PaintCollectible: No se encontró la instancia del ColorManager. Asegúrate de que el GameObject 'GameManager' con el script 'ColorManager.cs' esté en la escena y activo.", this);
            }

            Destroy(gameObject); // Destruye este objeto de pintura después de ser recolectado
        }
    }
}