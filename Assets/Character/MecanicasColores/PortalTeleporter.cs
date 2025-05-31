using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    [Tooltip("El punto de salida al que se moverá el jugador")]
    public Transform puntoDeSalida;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                // Desactivar momentáneamente para evitar conflictos de colisión
                controller.enabled = false;
                other.transform.position = puntoDeSalida.position;

                // Reactivar el controlador
                controller.enabled = true;

                Debug.Log("🌀 Jugador teletransportado");
            }
        }
    }
}
