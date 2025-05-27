using UnityEngine;
using UnityEngine.UI; // Necesario para usar componentes de UI como Button, GameObject, etc.

public class CircleColorMenu : MonoBehaviour
{
    [Header("Referencias UI (¡Arrastra y Asigna en Inspector!)")]
    [Tooltip("El GameObject Panel que contiene todos los botones del menú. Se activará/desactivará.")]
    public GameObject colorMenuPanel;

    [Tooltip("El botón para seleccionar el color Rojo.")]
    public Button redButton;

    [Tooltip("El botón para seleccionar el color Azul.")]
    public Button blueButton;

    [Tooltip("El botón para seleccionar el color Amarillo.")]
    public Button yellowButton;

    [Header("Configuración del Menú")]
    [Tooltip("La tecla que el jugador presionará para mostrar u ocultar el menú de colores.")]
    public KeyCode toggleKey = KeyCode.Q; // Tecla por defecto para alternar el menú

    private bool menuIsOpen = false; // Estado actual del menú (abierto o cerrado)

    void Start()
    {
        // Verificación inicial: Si no hay un panel asignado, el script no puede funcionar
        if (colorMenuPanel == null)
        {
            Debug.LogError("CircleColorMenu: ¡Error! El campo 'Color Menu Panel' no está asignado en el Inspector. Arrastra el GameObject del panel aquí.", this);
            enabled = false; // Deshabilita este script para evitar errores futuros
            return;
        }

        // Asegúrate de que el menú esté oculto cuando el juego comienza
        colorMenuPanel.SetActive(false);
        menuIsOpen = false;

        // ---- CONEXIÓN DE BOTONES PROGRAMÁTICAMENTE (¡Esto es CLAVE y es donde estaban los problemas!) ----
        // Aquí conectamos el evento 'onClick' de cada botón a nuestra función 'OnColorSelected'.
        // Usamos 'RemoveAllListeners()' para limpiar cualquier conexión anterior, evitando duplicados.

        if (redButton != null)
        {
            redButton.onClick.RemoveAllListeners(); // Limpia listeners existentes
            // Añade un nuevo listener: cuando el botón rojo se haga clic, llama a OnColorSelected con el primer color del ColorManager
            redButton.onClick.AddListener(() => OnColorSelected(ColorManager.Instance.availableColors[0]));
        }
        else Debug.LogWarning("CircleColorMenu: El botón rojo no está asignado. La interacción con el color rojo no funcionará.");

        if (blueButton != null)
        {
            blueButton.onClick.RemoveAllListeners();
            // Llama a OnColorSelected con el segundo color del ColorManager (azul)
            blueButton.onClick.AddListener(() => OnColorSelected(ColorManager.Instance.availableColors[1]));
        }
        else Debug.LogWarning("CircleColorMenu: El botón azul no está asignado. La interacción con el color azul no funcionará.");

        if (yellowButton != null)
        {
            yellowButton.onClick.RemoveAllListeners();
            // Llama a OnColorSelected con el tercer color del ColorManager (amarillo)
            yellowButton.onClick.AddListener(() => OnColorSelected(ColorManager.Instance.availableColors[2]));
        }
        else Debug.LogWarning("CircleColorMenu: El botón amarillo no está asignado. La interacción con el color amarillo no funcionará.");
    }

    void Update()
    {
        // Detecta si la tecla configurada (por defecto 'Q') es presionada
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMenu(); // Llama a la función para mostrar/ocultar el menú
        }
    }

    // Función para mostrar u ocultar el panel del menú
    public void ToggleMenu()
    {
        menuIsOpen = !menuIsOpen; // Invierte el estado del menú
        colorMenuPanel.SetActive(menuIsOpen); // Activa o desactiva el panel según el estado

        // Opcional: Puedes añadir lógica aquí para pausar el juego o deshabilitar el movimiento del jugador
        // cuando el menú está abierto para que no interfiera con la selección de color.
        // Ejemplo: Time.timeScale = menuIsOpen ? 0f : 1f; (pausa/despausa el tiempo del juego)
    }

    // Función que se llama cuando se hace clic en uno de los botones de color del menú
    public void OnColorSelected(Color selectedColor)
    {
        // Verifica si el ColorManager existe antes de intentar cambiar el color
        if (ColorManager.Instance != null)
        {
            ColorManager.Instance.SetBackgroundColor(selectedColor); // Llama al ColorManager para cambiar el fondo
        }
        else
        {
            Debug.LogError("CircleColorMenu: No se encontró la instancia del ColorManager. ¿Está el GameObject 'GameManager' en la escena con el script 'ColorManager.cs' adjunto?", this);
        }

        ToggleMenu(); // Oculta el menú después de seleccionar un color
    }
}