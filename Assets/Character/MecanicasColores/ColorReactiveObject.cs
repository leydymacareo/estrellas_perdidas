using UnityEngine;

[RequireComponent(typeof(Renderer))] // Asegura que el GameObject siempre tenga un componente Renderer (para mostrarse)
public class ColorReactiveObject : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    [Tooltip("El color intrínseco de este objeto. ¡Debe coincidir con uno de los colores definidos en el ColorManager y configurado con valores RGB exactos (0 o 255) y Alfa en 255!")]
    public Color objectColor; // El color de este objeto (ej. Rojo, Azul, Amarillo)

    private Renderer objectRenderer; // Referencia al componente Renderer del objeto
    private Collider objectCollider; // Referencia al componente Collider del objeto (para habilitar/deshabilitar interacción)

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>(); // Obtiene el Renderer
        objectCollider = GetComponent<Collider>(); // Intenta obtener un Collider (podría ser nulo si no hay)
        Debug.Log($"Collider {this.name}: {objectCollider.name}");

        if (objectRenderer == null)
        {
            Debug.LogError("ColorReactiveObject: ¡Error! No se encontró un Renderer en este GameObject (" + name + "). El script se deshabilitará.", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        // Suscríbete al evento del ColorManager para ser notificado cuando el fondo cambie
        ColorManager.onBackgroundColorChange += OnBackgroundColorChanged;
    }

    void OnDisable()
    {
        // Desuscríbete del evento cuando el objeto se desactive o se destruya para evitar errores
        ColorManager.onBackgroundColorChange -= OnBackgroundColorChanged;
    }

    void Start()
    {
        // Establece el color visual del objeto a su color original al inicio del juego
        ApplyObjectColor(objectColor);

        Debug.Log($"Start {this.name}");
        Debug.Log($"Start {this.name} {ColorManager.Instance != null}, {ColorManager.Instance.mainCamera != null}, {ColorManager.Instance.mainCamera.clearFlags == CameraClearFlags.SolidColor}");
        Debug.Log($"{ColorManager.Instance.mainCamera.name}");

        // Asegúrate de que el objeto reaccione al color de fondo actual si el ColorManager ya está inicializado
        // Importante: Al inicio el fondo será Skybox, por lo que los objetos estarán visibles.
        // Solo reacciona si el fondo ya es un color sólido (no Skybox)
        if (ColorManager.Instance != null && ColorManager.Instance.mainCamera != null && ColorManager.Instance.mainCamera.clearFlags == CameraClearFlags.Skybox)
        {
            OnBackgroundColorChanged(ColorManager.Instance.currentColor);
        }
    }

    // Método que se llama cuando el color de fondo cambia (gracias al evento del ColorManager)
    private void OnBackgroundColorChanged(Color newBackgroundColor)
    {
        Debug.Log($"OnBackgroundColorChanged {this.name} {newBackgroundColor}");
        // Compara el color del objeto con el nuevo color de fondo (usando una pequeña tolerancia por si acaso)
        // Pero es fundamental que los valores de color en el Inspector sean EXACTOS (0 o 255).
        if (ColorManager.Instance != null && ColorManager.Instance.ColorsAreApproximatelyEqual(objectColor, newBackgroundColor, 0.01f)) // Ahora usa la función pública
        {
            // Los colores coinciden: el objeto se fusiona con el fondo (se "desaparece" visualmente)
            ApplyObjectColor(newBackgroundColor); // Establece el color del objeto igual al color de fondo
            if (objectCollider != null) objectCollider.enabled = false; // Deshabilita el collider para que el jugador pueda pasar
            Debug.Log(name + " (Color: " + objectColor.ToString() + ") ha desaparecido."); // Mensaje de depuración
        }
        else
        {
            // Los colores no coinciden: el objeto vuelve a ser visible con su color original
            ApplyObjectColor(objectColor); // Restaura el color original del objeto
            if (objectCollider != null) objectCollider.enabled = true; // Habilita el collider para que el jugador no pueda pasar
            Debug.Log(name + " (Color: " + objectColor.ToString() + ") ha aparecido."); // Mensaje de depuración
        }
    }

    // Aplica el color deseado al material del Renderer del objeto
    private void ApplyObjectColor(Color targetColor)
    {
        if (objectRenderer != null && objectRenderer.material != null)
        {
            // Comprueba las propiedades de color más comunes en los shaders de Unity
            if (objectRenderer.material.HasProperty("_BaseColor")) // Usado comúnmente en URP/HDRP Lit/Standard
            {
                objectRenderer.material.SetColor("_BaseColor", targetColor);
            }
            else if (objectRenderer.material.HasProperty("_Color")) // Usado comúnmente en Built-in Standard/Unlit
            {
                objectRenderer.material.SetColor("_Color", targetColor);
            }
            else
            {
                Debug.LogWarning("ColorReactiveObject: El material de '" + name + "' no tiene la propiedad '_BaseColor' ni '_Color'. El cambio de color podría no verse correctamente. Asegúrate de que el shader del material permita cambiar el color base (ej. Standard o Lit).", this);
            }
        }
    }

    // Función auxiliar para comparar dos colores con una pequeña tolerancia (necesario por la precisión de los flotantes)
    // Ya no es necesaria aquí, se usa la del ColorManager
    // private bool ColorsAreApproximatelyEqual(Color c1, Color c2, float tolerance)
    // {
    //     return Mathf.Abs(c1.r - c2.r) < tolerance &&
    //            Mathf.Abs(c1.g - c2.g) < tolerance &&
    //            Mathf.Abs(c1.b - c2.b) < tolerance &&
    //            Mathf.Abs(c1.a - c2.a) < tolerance;
    // }
}