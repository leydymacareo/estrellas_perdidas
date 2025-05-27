using UnityEngine;
using System.Collections.Generic; // Necesario para List

public class ColorManager : MonoBehaviour
{
    // Implementación del patrón Singleton para acceder fácilmente a este script desde otros lugares
    public static ColorManager Instance { get; private set; }

    [Header("Configuración de Colores")]
    [Tooltip("Lista de colores disponibles para el juego. El primer color será el inicial.")]
    public List<Color> availableColors = new List<Color>();

    [Tooltip("Arrastra tu Main Camera aquí. Se buscará automáticamente si tiene el tag 'MainCamera'.")]
    public Camera mainCamera; 

    [Header("Color Actual")]
    [Tooltip("El color de fondo actual de la cámara.")]
    public Color currentColor;

    // Evento que otros scripts pueden 'escuchar' para saber cuándo cambia el color de fondo
    public delegate void OnBackgroundColorChange(Color newColor);
    public static event OnBackgroundColorChange onBackgroundColorChange;

    private CameraClearFlags originalClearFlags; // Para guardar el modo de Clear Flags original de la cámara

    private void Awake()
    {
        // Lógica del Singleton: asegura que solo haya una instancia de ColorManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye esta nueva instancia si ya existe una
        }
        else
        {
            Instance = this; // Establece esta instancia como la única
        }

        // Intenta encontrar la cámara principal si no se asignó manualmente en el Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Busca la cámara con el tag "MainCamera"
            if (mainCamera == null)
            {
                Debug.LogError("ColorManager: ¡Advertencia! No se encontró la cámara principal (con el tag 'MainCamera'). Asígnala manualmente en el Inspector para que el cambio de fondo funcione.", this);
            }
        }

        if (mainCamera != null)
        {
            originalClearFlags = mainCamera.clearFlags; // Guarda el Clear Flags original de la cámara
        }
    }

    void Start()
    {
        // ¡IMPORTANTE! No llamamos a SetBackgroundColor aquí para mantener el Skybox por defecto.
        // El color de fondo solo cambiará cuando el jugador lo seleccione desde la UI.

        // Solo verificamos que haya colores disponibles para el futuro uso de la UI
        if (availableColors.Count == 0)
        {
            Debug.LogWarning("ColorManager: No hay colores configurados en la lista 'Available Colors'. Añade al menos un color para que la selección funcione.");
        }
    }

    // Método público para cambiar el color de fondo de la cámara
    public void SetBackgroundColor(Color newColor)
    {
        currentColor = newColor; // Actualiza el color actual del manager

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor; // Cambia a color sólido
            mainCamera.backgroundColor = newColor; // Establece el nuevo color de fondo
        }
        else
        {
            Debug.LogError("ColorManager: mainCamera es nula al intentar cambiar el color de fondo.", this);
        }

        // Dispara el evento para que los objetos reactivos puedan actualizarse
        if (onBackgroundColorChange != null)
        {
            onBackgroundColorChange(newColor);
        }
    }

    // Método para restaurar el Skybox original (puedes llamarlo desde un botón UI si quieres)
    public void RestoreSkybox()
    {
        if (mainCamera != null)
        {
            mainCamera.clearFlags = originalClearFlags; // Restaura el Clear Flags original (debería ser Skybox)
        }
    }

    // *** (Opcional) Método de prueba con el teclado. Puedes eliminarlo una vez que la UI funcione perfectamente. ***
    void Update()
    {
        if (availableColors.Count > 0 && Input.GetKeyDown(KeyCode.Alpha1)) // Tecla '1'
        {
            SetBackgroundColor(availableColors[0]); // Cambia al primer color
        }
        if (availableColors.Count > 1 && Input.GetKeyDown(KeyCode.Alpha2)) // Tecla '2'
        {
            SetBackgroundColor(availableColors[1]); // Cambia al segundo color
        }
        if (availableColors.Count > 2 && Input.GetKeyDown(KeyCode.Alpha3)) // Tecla '3'
        {
            SetBackgroundColor(availableColors[2]); // Cambia al tercer color
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) // Tecla '0' para restaurar Skybox
        {
            RestoreSkybox();
        }
    }
    // ************************************************************************************************************
}
