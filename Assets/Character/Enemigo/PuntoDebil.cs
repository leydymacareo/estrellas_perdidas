using UnityEngine;

public class PuntoDebil : MonoBehaviour
{
    public ManchaEnemiga enemigo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("☠️ Punto débil tocado. Mancha destruida.");

            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.Rebotar();
            }

            if (enemigo != null)
                enemigo.Destruir();
        }
    }
}
