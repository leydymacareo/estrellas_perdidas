using UnityEngine;

public class GirasolVida : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LiveSystem vidas = other.GetComponent<LiveSystem>();
            if (vidas != null)
            {
                vidas.AgregarVida();
            }

            Destroy(gameObject); // Desaparecer el girasol
        }
    }
}
