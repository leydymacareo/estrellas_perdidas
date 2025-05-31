using UnityEngine;

public class DontDestroy : MonoBehaviour // <-- ¡El nombre de la clase debe coincidir con el nombre del archivo!
{
    // Esto es un patrón "Singleton" simple para asegurar que solo haya una instancia de este objeto.
    // Ayuda a evitar duplicados si el objeto persistente es también el objeto del jugador.
    public static DontDestroy Instance { get; private set; }

    void Awake()
    {
        // Verifica si ya existe otra instancia de esta clase (otro objeto persistente)
        // Y si esta instancia actual no es la que ya está establecida como la principal.
        if (Instance != null && Instance != this)
        {
            // Si ya hay una instancia persistente del jugador, destruye esta nueva instancia duplicada.
            Destroy(gameObject);
            Debug.Log("DontDestroy: Instancia duplicada de '" + gameObject.name + "' detectada y destruida. El objeto persistente original continúa.");
        }
        else
        {
            // Si esta es la primera o única instancia, la establecemos como la principal.
            Instance = this;

            // Esto es lo CRUCIAL: le dice a Unity que este GameObject (el que tiene este script)
            // NO debe ser destruido cuando se carga una nueva escena.
            DontDestroyOnLoad(gameObject);
            Debug.Log("DontDestroy: El objeto '" + gameObject.name + "' se configuró para NO ser destruido al cargar la escena.");
        }
    }

    // Nota: Aunque este script hace que el GameObject persista, si este GameObject
    // tiene otros scripts que referencian objetos específicos de la escena (ej. Main Camera,
    // o un suelo para el ground check), esos scripts aún necesitarán re-obtener esas referencias
    // cuando una nueva escena se carga (ej. usando SceneManager.sceneLoaded o GameObject.Find).
    // Ya hicimos esto en el ColorManager y te di indicaciones para tu script de movimiento.
}