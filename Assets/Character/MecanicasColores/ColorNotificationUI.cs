using UnityEngine;
using TMPro; // Necesario para TextMeshPro (si tu componente de texto es TextMeshProUGUI)
using System.Collections; // Necesario para Coroutines (para el temporizador)

public class ColorNotificationUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("El GameObject del panel de notificación UI que se mostrará/ocultará.")]
    public GameObject notificationPanel;

    [Tooltip("El componente de texto (TextMeshPro) donde se mostrará el mensaje de notificación.")]
    public TextMeshProUGUI notificationText; // Usa TextMeshProUGUI si tu texto es TextMeshPro. Si usas Text legacy, cámbialo a 'public UnityEngine.UI.Text notificationText;'

    [Tooltip("Duración que la notificación permanecerá visible en segundos.")]
    public float displayDuration = 3f;

    private Coroutine displayCoroutine; // Para controlar la corutina de ocultar la notificación

    void Awake()
    {
        // Verifica si las referencias UI están asignadas
        if (notificationPanel == null)
        {
            Debug.LogError("ColorNotificationUI: 'notificationPanel' no asignado en el Inspector. La notificación no funcionará.", this);
        }
        if (notificationText == null)
        {
            Debug.LogError("ColorNotificationUI: 'notificationText' no asignado en el Inspector. La notificación no funcionará.", this);
        }

        // Asegura que el panel de notificación esté oculto al inicio del juego
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        // Suscríbete al evento 'onColorUnlocked' del ColorManager para ser notificado
        ColorManager.onColorUnlocked += DisplayColorNotification;
    }

    void OnDisable()
    {
        // Desuscríbete del evento cuando este GameObject se desactiva o destruye
        ColorManager.onColorUnlocked -= DisplayColorNotification;
        // Detiene la corutina si está corriendo para evitar errores al deshabilitar el objeto
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
    }

    // Método que se llama cuando un color es desbloqueado (disparado por ColorManager)
    void DisplayColorNotification(ColorManager.AvailableGameColor unlockedColor)
    {
        if (notificationPanel == null || notificationText == null)
        {
            Debug.LogWarning("ColorNotificationUI: Referencias UI nulas, no se puede mostrar la notificación.", this);
            return;
        }

        // Si ya hay una notificación mostrándose, la detiene para mostrar la nueva
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        // Construye el texto de la notificación
        notificationText.text = "¡Color Desbloqueado!\n" +
                                unlockedColor.colorName.ToUpper() + " [" + GetKeyString(unlockedColor.hotkey) + "]";
        
        // Opcional: Cambia el color del texto al color que fue desbloqueado
        notificationText.color = unlockedColor.color;

        notificationPanel.SetActive(true); // Muestra el panel de notificación

        // Inicia una corutina para ocultar la notificación después de un tiempo
        displayCoroutine = StartCoroutine(HideNotificationAfterDelay());
    }

    // Corutina para esperar y luego ocultar el panel de notificación
    IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration); // Espera la duración configurada
        notificationPanel.SetActive(false); // Oculta el panel
        displayCoroutine = null; // Reinicia la referencia de la corutina
    }

    // Función auxiliar para obtener un string legible de un KeyCode (ej. Alpha1 -> "1")
    private string GetKeyString(KeyCode key)
    {
        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
        {
            return ((int)(key - KeyCode.Alpha0)).ToString(); // Convierte Alpha1 a "1"
        }
        return key.ToString(); // Para otras teclas (ej. "Space", "E", "F")
    }
}