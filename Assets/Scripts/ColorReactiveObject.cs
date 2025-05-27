using UnityEngine;

[RequireComponent(typeof(Renderer))] // Asegura que el GameObject siempre tenga un componente Renderer (para mostrarse)
public class ColorReactiveObject : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    [Tooltip("El color intrínseco de este objeto. Debe coincidir con uno de los colores del ColorManager.")]
    public Color objectColor; // El color de este objeto (Rojo, Azul, Amarillo)

    private Renderer objectRenderer; // Referencia al componente Renderer del objeto
    private Collider objectCollider; // Referencia al componente Collider del objeto (para habilitar/deshabilitar interacción)

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>(); // Obtiene el Renderer
        objectCollider = GetComponent<Collider>(); // Intenta obtener un Collider (podría ser nulo si no hay)

        if (objectRenderer == null)
        {
            Debug.LogError("ColorReactiveObject: ¡Error! No se encontró un Renderer en este GameObject (" + name + "). El script se deshabilitará.", this);
            enabled = false; // Deshabilita el script si no hay Renderer
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

        // Asegúrate de que el objeto reaccione al color de fondo actual si el ColorManager ya está inicializado
        if (ColorManager.Instance != null)
        {
            OnBackgroundColorChanged(ColorManager.Instance.currentColor);
        }
    }

    // Método que se llama cuando el color de fondo cambia (gracias al evento del ColorManager)
    private void OnBackgroundColorChanged(Color newBackgroundColor)
    {
        // Compara el color del objeto con el nuevo color de fondo (usando una pequeña tolerancia por si acaso)
        if (ColorsAreApproximatelyEqual(objectColor, newBackgroundColor, 0.01f))
        {
            // Los colores coinciden: el objeto se fusiona con el fondo (se "desaparece" visualmente)
            ApplyObjectColor(newBackgroundColor); // Establece el color del objeto igual al color de fondo
            if (objectCollider != null) objectCollider.enabled = false; // Deshabilita el collider para que el jugador pueda pasar
        }
        else
        {
            // Los colores no coinciden: el objeto vuelve a ser visible con su color original
            ApplyObjectColor(objectColor); // Restaura el color original del objeto
            if (objectCollider != null) objectCollider.enabled = true; // Habilita el collider para que el jugador no pueda pasar
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
                Debug.LogWarning("ColorReactiveObject: El material de '" + name + "' no tiene la propiedad '_BaseColor' ni '_Color'. El cambio de color podría no verse correctamente.", this);
            }
        }
    }

    // Función auxiliar para comparar dos colores con una pequeña tolerancia (útil para flotantes)
    private bool ColorsAreApproximatelyEqual(Color c1, Color c2, float tolerance)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}