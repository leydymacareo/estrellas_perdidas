using UnityEngine;

public class PuntoDebil : MonoBehaviour
{
    public GameObject ENEMY;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("☠️ Punto débil tocado. Mancha destruida.");

            // Activar inmunidad temporal al jugador
            LiveSystem vidas = other.GetComponentInParent<LiveSystem>();
            if (vidas != null)
            {
                vidas.ActivarInmunidadCorta();
            }

            Destroy(ENEMY);
        }
    }

}
