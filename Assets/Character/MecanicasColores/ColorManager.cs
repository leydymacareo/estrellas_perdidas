using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // <-- ¡Asegúrate de que esta línea está!

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [System.Serializable]
    public class AvailableGameColor
    {
        public Color color;
        public string colorName;
        public KeyCode hotkey;
        [HideInInspector]
        public bool isUnlocked = false;
    }

    [Header("Configuración de Colores del Juego")]
    [Tooltip("Define todos los colores posibles en el juego con su nombre y la tecla de acceso rápido. ¡Configura esto en el Inspector con valores RGB exactos (0 o 255) y Alfa en 255!")]
    public List<AvailableGameColor> gameColors = new List<AvailableGameColor>();

    public Camera mainCamera;
    public Color currentColor;

    public delegate void OnBackgroundColorChange(Color newColor);
    public static event OnBackgroundColorChange onBackgroundColorChange;

    public delegate void OnColorUnlocked(AvailableGameColor unlockedColor);
    public static event OnColorUnlocked onColorUnlocked;

    private CameraClearFlags originalClearFlags;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }

        // No buscar la cámara aquí si va a ser reasignada en OnSceneLoaded.
        // La asignación en el Inspector es la principal, o se buscará tras cargar escena.

        // Guardar el Clear Flags original, asumiendo que la cámara será la misma al inicio.
        // Esto se actualizará en OnSceneLoaded si la cámara cambia.
        if (mainCamera != null) { originalClearFlags = mainCamera.clearFlags; }
    }

    void OnEnable()
    {
        // Suscribirse al evento de carga de escena para re-obtener referencias
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Desuscribirse del evento al deshabilitar/destruir el GameObject
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se llama cada vez que se carga una escena.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("ColorManager: Escena cargada: " + scene.name + ". Re-verificando cámara principal.");

        // Re-buscar la cámara principal si la referencia actual es nula o si es la de la escena anterior.
        // 'Camera.main' siempre devuelve la cámara con el tag 'MainCamera' en la escena activa.
        if (mainCamera == null || mainCamera.gameObject.scene != scene) // Verifica si la cámara actual no es la de la nueva escena
        {
            mainCamera = Camera.main; // Busca la cámara principal en la escena recién cargada.
            if (mainCamera == null)
            {
                Debug.LogWarning("ColorManager: La cámara principal no se encontró en la escena " + scene.name + ". Asegúrate de que tenga el Tag 'MainCamera'.");
            }
            else
            {
                // Si la cámara se encontró de nuevo, actualiza el originalClearFlags
                originalClearFlags = mainCamera.clearFlags;
                Debug.Log("ColorManager: Nueva cámara principal encontrada y asignada en la escena " + scene.name + ".");
            }
        }

        // Si el fondo era sólido en la escena anterior, asegúrate de que la nueva cámara lo muestre
        // o que la lógica de cambio de color se active de nuevo.
        // Aquí puedes decidir si quieres que el color de fondo persista o empiece con Skybox.
        // Si el ColorManager está en modo Skybox al inicio, no necesitas hacer nada aquí.
    }

    void Start()
    {
        foreach (var col in gameColors)
        {
            col.isUnlocked = false;
        }
        if (gameColors.Count == 0)
        {
            Debug.LogWarning("ColorManager: No hay colores configurados en la lista 'Game Colors' en el Inspector. Añade al menos un color para que la selección funcione.");
        }
    }

    void Update()
    {
        foreach (var gameColor in gameColors)
        {
            if (gameColor.isUnlocked && Input.GetKeyDown(gameColor.hotkey))
            {
                SetBackgroundColor(gameColor.color);
                Debug.Log("Fondo cambiado a " + gameColor.colorName + " por hotkey: [" + gameColor.hotkey.ToString() + "].");
            }
        }
    }

    public void SetBackgroundColor(Color newColor)
    {
        currentColor = newColor;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = newColor;
        }
        else
        {
            Debug.LogError("ColorManager: La referencia a 'mainCamera' es nula al intentar cambiar el color de fondo. Asegúrate de asignarla en el Inspector y que la cámara en la escena tenga el Tag 'MainCamera'.", this);
        }
        if (onBackgroundColorChange != null) { onBackgroundColorChange(newColor); }
    }

    public void RestoreSkybox()
    {
        if (mainCamera != null) { mainCamera.clearFlags = originalClearFlags; }
        else
        {
            Debug.LogError("ColorManager: La referencia a 'mainCamera' es nula al intentar restaurar el Skybox.", this);
        }
    }

    public void UnlockColor(Color colorToUnlock)
    {
        foreach (var gameColor in gameColors)
        {
            if (ColorsAreApproximatelyEqual(gameColor.color, colorToUnlock, 0.01f))
            {
                if (!gameColor.isUnlocked)
                {
                    gameColor.isUnlocked = true;
                    Debug.Log("¡Color " + gameColor.colorName + " DESBLOQUEADO! Puedes usar la tecla [" + gameColor.hotkey.ToString() + "].");
                    if (onColorUnlocked != null) { onColorUnlocked(gameColor); }
                    return;
                }
                else
                {
                    Debug.Log("Color " + gameColor.colorName + " ya estaba desbloqueado.");
                }
            }
        }
        Debug.LogWarning("ColorManager: Se intentó desbloquear un color no definido en la lista 'Game Colors' O los valores de color no coinciden EXACTAMENTE: " + colorToUnlock.ToString() + ". Por favor, verifica que los valores RGB y Alfa sean idénticos (0 o 255) en ColorManager y PaintCollectible.");
    }

    public bool ColorsAreApproximatelyEqual(Color c1, Color c2, float tolerance)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}