using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CarAI : MonoBehaviour
{
    private NavMeshAgent carAgent;

    [Header("Route")]
    public Transform[] startRoute;
    public int currentWaypointIndex = 0;
    public Transform[] currentRoute;

    [Header("Routes")]
    public Transform[] route_1;
    public Transform[] route_2;
    public Transform[] route_3;
    public Transform[] route_4;

    [Header("Traffic Lights")]
    public StopLight[] routeTrafficLights;

    private StopLight currentTrafficLight;

    [Header("Speed")]
    public float normalSpeed = 5f;

    [Header("Raycast Car Detection")]
    public Transform rayOrigin;
    public float rayDistance = 4f;
    public LayerMask carLayer;
    public bool carInFront = false;

    private bool routeChosen = false;

    private void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.speed = normalSpeed;
        carAgent.autoBraking = true;

        currentRoute = startRoute;
        currentWaypointIndex = 0;
        MoveToCurrentWaypoint();
    }

    private void Update()
    {
        CheckCarInFront();

        if (carInFront)
        {
            carAgent.isStopped = true;
            return;
        }

        CheckTrafficLightStop();

        if (!carAgent.isStopped && !carAgent.pathPending)
        {
            if (carAgent.remainingDistance <= carAgent.stoppingDistance + 0.2f)
            {
                GoToNextWaypoint();
            }
        }
    }

    private void CheckTrafficLightStop()
    {
        if (currentTrafficLight == null)
        {
            carAgent.isStopped = false;
            carAgent.speed = normalSpeed;
            return;
        }

        if (!currentTrafficLight.isGreen)
        {
            carAgent.isStopped = true;
            carAgent.speed = 0f;
            // Debug.Log("<color=red>[Car] FORCE STOP - RED LIGHT</color>");
        }
        else
        {
            carAgent.isStopped = false;
            carAgent.speed = normalSpeed;
        }
    }

    private void MoveToCurrentWaypoint()
    {
        if (currentRoute == null || currentRoute.Length == 0) return;
        carAgent.SetDestination(currentRoute[currentWaypointIndex].position);
    }

    private void GoToNextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= currentRoute.Length)
        {
            if (!routeChosen)
            {
                // Finished startRoute → choose a main route
                ChooseRandomRoute();
            }
            else
            {
                // Finished the chosen main route → DESTROY the car
                DestroyCar();
            }
        }
        else
        {
            MoveToCurrentWaypoint();
        }
    }

    public void ChooseRandomRoute()
    {
        routeChosen = true;
        int randomRoute = Random.Range(0, 4);

        switch (randomRoute)
        {
            case 0: currentRoute = route_1; SetCurrentTrafficLight(0); break;
            case 1: currentRoute = route_2; SetCurrentTrafficLight(1); break;
            case 2: currentRoute = route_3; SetCurrentTrafficLight(2); break;
            case 3: currentRoute = route_4; SetCurrentTrafficLight(3); break;
        }

        currentWaypointIndex = 0;
        MoveToCurrentWaypoint();
    }

    private void SetCurrentTrafficLight(int routeIndex)
    {
        currentTrafficLight = (routeTrafficLights != null && routeIndex < routeTrafficLights.Length)
                            ? routeTrafficLights[routeIndex] : null;

        // Debug.Log($"[Car] Assigned traffic light for route {routeIndex}");
    }

    private void CheckCarInFront()
    {
        carInFront = false;
        if (rayOrigin == null) return;

        Vector3 origin = rayOrigin.position;
        Vector3 direction = transform.forward;
        Debug.DrawRay(origin, direction * rayDistance, Color.yellow);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, carLayer))
        {
            carInFront = true;
        }
    }

    private void DestroyCar()
    {
        Debug.Log($"<color=orange>[Car] Reached end of route - Destroying {gameObject.name}</color>");
        Destroy(gameObject);
    }

    public void AssignSceneReferences(Transform[] start, Transform[] r1, Transform[] r2, Transform[] r3, Transform[] r4,
                                      StopLight[] trafficLights, Transform[] stopPoints)
    {
        startRoute = start;
        route_1 = r1;
        route_2 = r2;
        route_3 = r3;
        route_4 = r4;

        routeTrafficLights = trafficLights;

        currentRoute = startRoute;
        currentWaypointIndex = 0;
        currentTrafficLight = null;
        routeChosen = false;
    }
}