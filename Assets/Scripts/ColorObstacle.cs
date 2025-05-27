using UnityEngine;

[RequireComponent(typeof(Renderer))] // Asegura que el objeto tenga un componente Renderer
public class ColorObstacle : MonoBehaviour
{
    [Header("Configuración del Obstáculo")]
    public Color obstacleColor; // El color intrínseco de este obstáculo

    private Renderer obstacleRenderer;
    private Material originalMaterial; // Para guardar el material original del obstáculo
    private Color originalMaterialColor; // Para guardar el color original del material

    void Awake()
    {
        obstacleRenderer = GetComponent<Renderer>();

        // Guarda el material original y su color base para restaurarlo
        // Es importante hacer una copia de la instancia del material si quieres que cada obstáculo sea independiente
        // Si tienes muchos obstáculos del mismo color y quieres que compartan el mismo material,
        // podrías simplemente guardar una referencia al material y cambiar su color directamente.
        // Para este ejemplo, haremos una copia para que cada obstáculo pueda tener su propio material modificado.
        originalMaterial = obstacleRenderer.material;
        originalMaterialColor = originalMaterial.color; // Asume que el color base está en _Color o albedoColor

        // Si estás usando URP/HDRP, el color base suele ser "_BaseColor" o similar
        if (originalMaterial.HasProperty("_BaseColor"))
        {
            originalMaterialColor = originalMaterial.color;
        }
        else if (originalMaterial.HasProperty("_Color"))
        {
            originalMaterialColor = originalMaterial.color;
        }
        else
        {
            Debug.LogWarning("ColorObstacle: El material del objeto " + name + " no tiene la propiedad '_Color' o '_BaseColor'. La lógica de cambio de color podría no funcionar correctamente.", this);
        }
    }

    void OnEnable()
    {
        // Suscríbete al evento de cambio de color de fondo
        ColorManager.onBackgroundColorChange += OnBackgroundColorChanged;
    }

    void OnDisable()
    {
        // Desuscríbete del evento cuando el objeto se desactiva o destruye
        ColorManager.onBackgroundColorChange -= OnBackgroundColorChanged;
    }

    void Start()
    {
        // Asegúrate de que el obstáculo tenga el color inicial correcto al inicio del juego
        // Esto es importante si el ColorManager ya ha establecido un color de fondo antes de que este objeto esté activo
        if (ColorManager.Instance != null)
        {
            OnBackgroundColorChanged(ColorManager.Instance.currentColor);
        }

        // Establece el color original del obstáculo en su material al inicio
        // Esto es crucial para que el obstáculo se vea de su color definido inicialmente
        if (obstacleRenderer.material != null)
        {
             if (obstacleRenderer.material.HasProperty("_BaseColor"))
            {
                obstacleRenderer.material.SetColor("_BaseColor", obstacleColor);
            }
            else if (obstacleRenderer.material.HasProperty("_Color"))
            {
                obstacleRenderer.material.SetColor("_Color", obstacleColor);
            }
        }
    }

    // Este método se llama cada vez que el color de fondo cambia
    private void OnBackgroundColorChanged(Color newBackgroundColor)
    {
        // Compara el color del obstáculo con el nuevo color de fondo
        // Usa una pequeña tolerancia para evitar problemas con la precisión de los flotantes
        if (ColorsAreApproximatelyEqual(obstacleColor, newBackgroundColor, 0.01f)) // 0.01f es una pequeña tolerancia
        {
            // Los colores coinciden, hacer el obstáculo "desaparecer" (fusionarse con el fondo)
            if (obstacleRenderer.material != null)
            {
                if (obstacleRenderer.material.HasProperty("_BaseColor"))
                {
                    obstacleRenderer.material.SetColor("_BaseColor", newBackgroundColor);
                }
                else if (obstacleRenderer.material.HasProperty("_Color"))
                {
                    obstacleRenderer.material.SetColor("_Color", newBackgroundColor);
                }
            }
            // También puedes deshabilitar el collider si quieres que el jugador pueda pasar a través de él
            // GetComponent<Collider>().enabled = false;
        }
        else
        {
            // Los colores no coinciden, hacer el obstáculo visible con su color original
            if (obstacleRenderer.material != null)
            {
                if (obstacleRenderer.material.HasProperty("_BaseColor"))
                {
                    obstacleRenderer.material.SetColor("_BaseColor", obstacleColor);
                }
                else if (obstacleRenderer.material.HasProperty("_Color"))
                {
                    obstacleRenderer.material.SetColor("_Color", obstacleColor);
                }
            }
            // Habilitar el collider si lo deshabilitaste antes
            // GetComponent<Collider>().enabled = true;
        }
    }

    // Helper para comparar colores con tolerancia
    private bool ColorsAreApproximatelyEqual(Color c1, Color c2, float tolerance)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}