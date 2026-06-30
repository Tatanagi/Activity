using UnityEngine;
using UnityEngine.UI;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Prefab")]
    public GameObject carPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Routes (From Hierarchy)")]
    public Transform[] startRoute;
    public Transform[] route_1;
    public Transform[] route_2;
    public Transform[] route_3;
    public Transform[] route_4;  // NEW: Route 4

    [Header("Traffic Lights & Stop Points (Per Route)")]
    public StopLight[] routeTrafficLights;   // Must match order: route_1, route_2, route_3, route_4
    public Transform[] routeStopPoints;      // Same order

    [Header("UI")]
    public Button spawnButton;

    private bool isButtonReady = true;

    private void Start()
    {
        SetupSpawnButton();
    }

    private void SetupSpawnButton()
    {
        if (spawnButton == null)
        {
            Debug.LogWarning("Spawn Button is not assigned!");
            return;
        }

        spawnButton.onClick.RemoveAllListeners();
        spawnButton.onClick.AddListener(SpawnRandomCar);
    }

    public void SpawnRandomCar()
    {
        if (!isButtonReady) return;
        if (carPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Car Prefab or Spawn Points not assigned!");
            return;
        }

        isButtonReady = false;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject newCar = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

        CarAI carAI = newCar.GetComponent<CarAI>();
        if (carAI != null)
        {
            carAI.AssignSceneReferences(startRoute, route_1, route_2, route_3, route_4,
                                        routeTrafficLights, routeStopPoints);

            Debug.Log($"Car spawned at {spawnPoint.name}");
        }
        else
        {
            Debug.LogError("CarAI component missing on prefab!");
        }

        Invoke(nameof(ResetButton), 0.2f);
    }

    private void ResetButton()
    {
        isButtonReady = true;
    }

    private void OnDestroy()
    {
        if (spawnButton != null)
            spawnButton.onClick.RemoveAllListeners();
    }
}