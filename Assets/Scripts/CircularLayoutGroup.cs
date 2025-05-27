using UnityEngine;
using UnityEngine.UI; // Necesario para RectTransform

// [ExecuteAlways] // Descomenta esta línea si quieres ver los botones reorganizarse en el editor sin tener que entrar en Play mode
public class CircularLayoutGroup : MonoBehaviour
{
    [Header("Configuración del Círculo")]
    [Tooltip("El radio del círculo en el que se distribuirán los elementos hijos.")]
    public float radius = 100f; 

    [Range(0, 360)]
    [Tooltip("El ángulo inicial (en grados) para el primer elemento (0° = derecha, 90° = arriba).")]
    public float startAngle = 90f; 

    [Range(0, 360)]
    [Tooltip("El arco total (en grados) que cubrirán los elementos (ej. 360 para un círculo completo).")]
    public float totalArc = 360f; 

    void OnEnable()
    {
        // Se llama cuando el objeto se habilita. Asegura que los elementos se posicionen.
        LayoutChildren();
    }

    void OnValidate() // Se llama en el editor cuando se modifican las variables públicas en el Inspector
    {
        // Útil para ver los cambios de diseño al ajustar los valores en el Inspector sin ejecutar el juego
        LayoutChildren();
    }

    // Función principal para posicionar los elementos hijos en un círculo
    public void LayoutChildren()
    {
        // Primero, cuenta cuántos hijos (RectTransform) hay para distribuir
        int childCount = 0;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null)
            {
                childCount++;
            }
        }

        if (childCount == 0) return; // Si no hay hijos, no hay nada que hacer

        // Calcula el ángulo de separación entre cada elemento
        float angleStep = totalArc / childCount;

        int currentChildIndex = 0;
        foreach (Transform child in transform)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            if (childRect == null) continue; // Solo procesa GameObjects que son elementos de UI (RectTransform)

            // Calcula el ángulo para el elemento actual
            // Restamos el ángulo para distribuir en sentido horario
            float currentAngle = startAngle - (angleStep * currentChildIndex);
            
            // Convierte el ángulo de grados a radianes (necesario para Mathf.Cos y Mathf.Sin)
            float angleRad = currentAngle * Mathf.Deg2Rad;

            // Calcula las coordenadas X e Y en el círculo
            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;

            // Establece la posición del elemento UI (relative al centro de su padre)
            childRect.anchoredPosition = new Vector2(x, y);
            currentChildIndex++;
        }
    }
}