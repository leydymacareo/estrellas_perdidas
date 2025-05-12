using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CollectibleManager : MonoBehaviour
{
    [Header("SecuenciaMovimiento")]
    public float amplitud = 0.25f;
    public float speed = 2f;
    public float rotationSpeed = 45f;

    [Header("SistemaRecoleccion")]
    public TextMeshProUGUI itemCounter;
    public int totalItemsScene = 2;
    public string collectibleTag = "Collectible";
    private static int itemsCollected = 0;

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startPosition = new Dictionary<Transform, Vector3>();

    void Start()
    {
        foreach (var obj in collectibles)
        {
            if (obj != null)
            {
                startPosition[obj] = obj.position;

                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                {
                    col = obj.gameObject.AddComponent<BoxCollider>();
                }
                col.isTrigger = true;

                if (obj.GetComponent<PlayerCollectiveDetector>() == null)
                {
                    obj.gameObject.AddComponent<PlayerCollectiveDetector>().Init(this);
                }
            }
        }

        UpdateCounterUI();
    }

    void Update()
    {
        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            Vector3 StartPos = startPosition[obj];
            float newY = StartPos.y + Mathf.Sin(Time.deltaTime * speed) * amplitud;
            obj.position = new Vector3(StartPos.x, newY, StartPos.z);
            obj.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Collect(Transform obj)
    {
        if (!collectibles.Contains(obj)) return;

        collectibles.Remove(obj);
        itemsCollected++;
        UpdateCounterUI();
        Destroy(obj.gameObject);
    }

    void UpdateCounterUI()
    {
        if (itemCounter != null)
        {
            itemCounter.text = $"{itemsCollected} / {totalItemsScene}";
        }
    }
}
