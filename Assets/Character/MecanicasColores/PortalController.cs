using UnityEngine;
using UnityEngine.SceneManagement; // Necesario si quieres cargar escenas
using System.Collections; // Necesario para Coroutines (si usas HideMessageAfterDelay)

public class PortalController : MonoBehaviour
{
    [Header("Configuración del Portal")]
    [Tooltip("El color que debe estar desbloqueado para poder usar este portal. ¡Debe coincidir EXACTAMENTE con uno de los colores en el ColorManager! (RGB 0 o 255, Alfa 255).")]
    public Color requiredColor = Color.yellow; // Variable pública para el color requerido

    [Tooltip("El nombre de la escena a cargar si el portal se usa con éxito. Asegúrate de añadirla a 'File > Build Settings > Scenes In Build'.")]
    public string sceneToLoad = "Level2Scene"; // Nombre de la escena destino

    [Header("Referencias UI (Opcional)")]
    [Tooltip("Arrastra aquí un panel UI con un mensaje de error (ej. 'Necesitas pintura X'). Se activará si el color no está desbloqueado.")]
    public GameObject colorRequiredMessageUI; // Panel para mostrar mensaje de color requerido

    void Awake()
    {
        // Asegúrate de que el mensaje UI esté desactivado al inicio
        if (colorRequiredMessageUI != null)
        {
            colorRequiredMessageUI.SetActive(false);
        }
    }

    // Detecta cuando el jugador entra en el área del portal (el Collider con Is Trigger)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter activado. Objeto que entró: " + other.name + ", Tag: " + other.tag); // Depuración: Se detectó algo

        // Asegúrate de que el GameObject que entró tiene el Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Jugador entró al portal!"); // Depuración: Es el jugador

            // Verifica si la instancia de ColorManager está disponible
            if (ColorManager.Instance != null)
            {
                // Busca la información del color requerido en la lista de colores del juego del ColorManager
                ColorManager.AvailableGameColor requiredColorInfo = null;
                foreach (var gameColor in ColorManager.Instance.gameColors)
                {
                    // Usa la función pública ColorsAreApproximatelyEqual del ColorManager
                    if (ColorManager.Instance.ColorsAreApproximatelyEqual(gameColor.color, requiredColor, 0.01f))
                    {
                        requiredColorInfo = gameColor; // Encontró la definición de nuestro color requerido
                        break; // Sale del bucle
                    }
                }

                // Si se encontró la definición del color requerido en el ColorManager
                if (requiredColorInfo != null)
                {
                    Debug.Log("Color requerido del portal encontrado en ColorManager: " + requiredColorInfo.colorName + ". ¿Está desbloqueado? " + requiredColorInfo.isUnlocked); // Depuración: Color definido

                    // ¡Verifica si ese color requerido está actualmente desbloqueado!
                    if (requiredColorInfo.isUnlocked)
                    {
                        Debug.Log("¡Color " + requiredColorInfo.colorName + " está DESBLOQUEADO! Intentando cargar escena..."); // Depuración: Color desbloqueado
                        
                        // ---- Lógica para usar el portal (teletransportar, cargar nueva escena, etc.) ----
                        // Asegúrate de que el nombre de la escena en el Inspector sea correcto
                        SceneManager.LoadScene(sceneToLoad); // Carga la escena especificada
                        Debug.Log("¡Carga de escena solicitada para: " + sceneToLoad + "!"); // Depuración: Se pidió la carga
                        // ----------------------------------------------------------------------------------
                    }
                    else
                    {
                        // El color no está desbloqueado, muestra un mensaje al jugador
                        Debug.Log("El color " + requiredColorInfo.colorName + " NO está desbloqueado. Necesitas la pintura " + requiredColorInfo.colorName.ToLower() + "."); // Depuración: No desbloqueado
                        if (colorRequiredMessageUI != null)
                        {
                            colorRequiredMessageUI.SetActive(true); // Muestra el mensaje UI
                            // Inicia una corrutina para ocultar el mensaje después de unos segundos
                            StartCoroutine(HideMessageAfterDelay(3f));
                        }
                    }
                }
                else
                {
                    // Este error indica que el 'requiredColor' que configuraste en el Inspector de este portal
                    // no existe en la lista 'Game Colors' del ColorManager (o no coincide exactamente).
                    Debug.LogError("PortalController: ¡Error de configuración! El color requerido (" + requiredColor.ToString() + ") no está definido en la lista 'Game Colors' del ColorManager. Asegúrate de que los valores RGB y Alfa sean idénticos (0 o 255) y que el color esté en la lista 'Game Colors'.", this); // Depuración: Color no definido
                }
            }
            else
            {
                Debug.LogError("PortalController: La instancia de ColorManager es nula. Asegúrate de que el GameObject 'GameManager' con el script 'ColorManager.cs' esté en la escena y activo.", this); // Depuración: ColorManager nulo
            }
        }
        else
        {
            Debug.Log("Objeto que entró no es el jugador. Tag: " + other.tag); // Depuración: No es el jugador
        }
    }

    // Detecta cuando el jugador sale del área del portal (para ocultar el mensaje de error si está activo)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (colorRequiredMessageUI != null)
            {
                colorRequiredMessageUI.SetActive(false); // Oculta el mensaje al salir del trigger
            }
        }
    }

    // Corutina para ocultar el mensaje UI de error después de un retraso
    System.Collections.IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado
        if (colorRequiredMessageUI != null)
        {
            colorRequiredMessageUI.SetActive(false); // Oculta el panel
        }
    }
}