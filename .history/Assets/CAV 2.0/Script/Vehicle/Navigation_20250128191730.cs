using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class Navigation : MonoBehaviour
{
    public CommunicationAgent comagent;
    public TrafficSystem trafficSystem;
    public List<Segment> Map;
    public SimConfig.VehicleType vehicleType;
    public List<Segment> path;

    public Vector3 currentPos;
    public Segment currentSegment;
    public Segment nextSegment;

    public Vehicle_AI thisAI;

    public CarAI carAI;

    private void Start()
    {
        // Initialize components and variables
        thisAI = GetComponent<Vehicle_AI>();
        comagent = GetComponent<CommunicationAgent>();

        // Get current position and segment
        currentPos = transform.position;
        currentSegment = comagent.getCurrentSegment(currentPos);

        if (currentSegment == null)
        {
            Debug.LogError("Failed to determine the current segment.");
            return;
        }

        Debug.Log($"Current segment: {currentSegment.id}");
        comagent.EnterSegment(currentSegment);

        // Generate initial path
        path = RandomPath(currentSegment);

        if (path.Count > 1)
        {
            nextSegment = path[1];
            path.RemoveAt(0); // Remove the current segment from the path
        }
        else
        {
            Debug.LogError("Initial path generation failed or has insufficient segments.");
        }

        // thisAI.currentSegment = currentSegment;
        carAI = GetComponent<CarAI>();
        carAI.SetPath(vectorpath(currentSegment));
        
    }

    public List<Vector3> vectorpath(Segment s){
        List<Vector3> path = new List<Vector3>();
        s.waypoints.ForEach(wp => path.Add(wp.transform.position));
        return path;
    }
    private void Update()
    {
        if (vehicleType == SimConfig.VehicleType.CAV && path.Count <= 1)
        {
            // Regenerate path for CAV vehicles if near the end of the current path
            path = RandomPath(currentSegment);

            if (path.Count > 1)
            {
                nextSegment = path[1];
                path.RemoveAt(0);
            }
            else
            {
                Debug.LogWarning("Path regeneration failed for CAV.");
            }
        }
    }

    public void OntoNextSegment()
    {
        // Move to the next segment
        if (path == null || path.Count < 1)
        {
            Debug.LogError("Path is empty. Cannot move to the next segment.");
            //TODO - Send some end of journey message
            return;
        }

        comagent.ExitSegment(currentSegment);
        comagent.EnterSegment(nextSegment);

        currentSegment = nextSegment;

        if (path.Count > 1)
        {
            nextSegment = path[1];
            path.RemoveAt(0);
        }
        else
        {
            Debug.LogWarning("Path is about to be exhausted.");
            path = RandomPath(currentSegment); // Regenerate path
            if (path.Count > 0)
                nextSegment = path[0];
        }

        thisAI.currentSegment = currentSegment;
    }

    // Random path generator
    public List<Segment> RandomPath(Segment currentSegment)
    {
        if (currentSegment == null || currentSegment.nextSegments == null || currentSegment.nextSegments.Count == 0)
        {
            Debug.LogError("Current segment is null or has no connected segments.");
            return new List<Segment>();
        }

        List<Segment> newPath = new List<Segment> { currentSegment };
        Segment tempSegment = currentSegment;

        for (int i = 0; i < 5; i++)
        {
            if (tempSegment.nextSegments == null || tempSegment.nextSegments.Count == 0)
            {
                Debug.LogWarning($"Segment {tempSegment.id} has no further connections. Path generation stopped.");
                break;
            }

            tempSegment = tempSegment.nextSegments[Random.Range(0, tempSegment.nextSegments.Count)];
            newPath.Add(tempSegment);
        }

        return newPath;
    }

        public Segment nextSegments()
    {
        //get the next segment
        currentSegment = nextSegment;
        //get the next segment
        nextSegment = path[path.IndexOf(currentSegment) + 1];
        return currentSegment;
    }
}
