using UnityEngine;

public class PedestrianAI : MonoBehaviour
{
    [Header("Crossing Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Traffic Lights")]
    public StopLight[] trafficLights;

    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float waitTimeAtEnd = 3f;

    private bool isCrossing = false;
    private bool atStart = true;
    private float waitTimer = 0f;

    private Vector3 targetPosition;

    private void Start()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
        }
    }

    private void Update()
    {
        // Safety check
        if (startPoint == null || endPoint == null)
            return;

        bool isSafeToCross = IsSafeToCross();

        if (!isCrossing)
        {
            if (atStart)
            {
                if (isSafeToCross)
                {
                    StartCrossing();
                }
            }
            else
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTimeAtEnd)
                {
                    ReturnToStart();
                }
            }
        }
        else
        {
            // Move towards target
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                walkSpeed * Time.deltaTime
            );

            // Check if reached destination
            if (Vector3.Distance(transform.position, targetPosition) <= 0.2f)
            {
                FinishCrossing();
            }
        }
    }

    private bool IsSafeToCross()
    {
        if (trafficLights == null || trafficLights.Length == 0)
            return true; // No lights = always cross

        foreach (var light in trafficLights)
        {
            if (light != null && !light.isGreen)
                return true; // At least one red light = safe to cross
        }

        return false;
    }

    private void StartCrossing()
    {
        isCrossing = true;
        atStart = false;
        waitTimer = 0f;
        targetPosition = endPoint.position;

        Debug.Log($"<color=cyan>[Pedestrian] Crossing to end point ({gameObject.name})</color>");
    }

    private void FinishCrossing()
    {
        isCrossing = false;
        transform.position = targetPosition; // Snap to exact position
        Debug.Log($"<color=green>[Pedestrian] Reached destination ({gameObject.name})</color>");
    }

    private void ReturnToStart()
    {
        isCrossing = true;
        atStart = true;
        waitTimer = 0f;
        targetPosition = startPoint.position;

        Debug.Log($"<color=cyan>[Pedestrian] Returning to start ({gameObject.name})</color>");
    }

    public void AssignReferences(Transform start, Transform end, StopLight[] lights)
    {
        startPoint = start;
        endPoint = end;
        trafficLights = lights;

        if (start != null)
        {
            transform.position = start.position;
        }

        atStart = true;
        isCrossing = false;
        waitTimer = 0f;

        Debug.Log($"<color=lime>[Pedestrian] References assigned successfully ({gameObject.name})</color>");
    }

    private void OnDestroy()
    {
        isCrossing = false;
    }
}