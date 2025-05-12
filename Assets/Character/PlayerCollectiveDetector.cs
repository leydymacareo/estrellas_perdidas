using UnityEngine;

public class PlayerCollectiveDetector : MonoBehaviour
{
    private CollectibleManager manager;

    public void Init(CollectibleManager manager)
    {
        this.manager = manager;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.Collect(transform);
        }
    }
}
