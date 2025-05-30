using UnityEngine;

public class PuntoDebil : MonoBehaviour
{
    public GameObject ENEMY;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("☠️ Punto débil tocado. Mancha destruida.");
            Destroy(ENEMY); // destruye la mancha completa
        }
    }
}
