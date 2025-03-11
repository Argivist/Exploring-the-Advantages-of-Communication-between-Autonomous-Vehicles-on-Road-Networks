using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using static Navigation;
[System.Serializable]
public class Vehicle
{
    //vehicle name, typ, start time, end time list depending on number o simulations
    public int vehicleId;
    public string vehicleName;


    public VehicleType vehicleType;
    public int startTime;
    public Vector3 startPos;
    public Vector3 endPos;
    public Waypoint wdir;
    public Segment destSegment;

    public Vehicle(int vehicle_id, string vehicleName, VehicleType vehicleType, int startTime, Vector3 startPos, Vector3 endPos, Waypoint wdir, Segment destSegment)
    {
        this.vehicleId = vehicle_id;
        this.vehicleName = vehicleName;
        this.vehicleType = vehicleType;
        this.startTime = startTime;
        this.startPos = startPos;
        this.endPos = endPos;
        this.wdir = wdir;
        this.destSegment = destSegment;

    }
}

[System.Serializable]
//waypoint with segment, waypoiint adn waypoint position
public class WaypointObject
{
    public Segment segment;
    public Waypoint currentWaypoint;
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;
    public Vector3 position;

    public WaypointObject(Segment segment, Waypoint current, Waypoint next, Waypoint previous, Vector3 position)
    {
        this.segment = segment;
        this.currentWaypoint = current;
        this.nextWaypoint = next;
        this.previousWaypoint = previous;
        this.position = position;
    }
    public bool hasNext()
    {
        return nextWaypoint != null;
    }
    public bool hasPrevious()
    {
        return previousWaypoint != null;
    }

}


public class SimulationConfigurer : MonoBehaviour
{
    [Header("Simulation Configurer")]

    public int VehicleDensity;
    public bool ready = false;

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;

    [Header("Vehicle List")]
    public List<Vehicle> vehicleList = new List<Vehicle>();

    [HideInInspector]
    public DataHandler dataHandler;



IEnumerator VConf()
    {
        // dataHandler = GetComponent<DataHandler>();

        // Get all available waypoints
        List<WaypointObject> waypointList = new List<WaypointObject>();
        foreach (Segment segment in trafficSystem.segments)
        {
            for (int i = 0; i < segment.waypoints.Count; i++)
            {
                Waypoint waypoint = segment.waypoints[i];
                Waypoint nextWaypoint = i < segment.waypoints.Count - 1 ? segment.waypoints[i + 1] : null;
                Waypoint previousWaypoint = i > 0 ? segment.waypoints[i - 1] : null;
                waypointList.Add(new WaypointObject(segment, waypoint, nextWaypoint, previousWaypoint, waypoint.transform.position));
            }
        }

        // Configure the cars
        for (int i = 0; i < VehicleDensity; i++)
        {
            // Randomly select a start waypoint
            int startWaypointIndex = Random.Range(0, waypointList.Count);
            Vector3 startPos = Vector3.zero;
            Waypoint dirPoint;
            Segment startSegment = waypointList[startWaypointIndex].segment;
            int destSegment = Random.Range(0, trafficSystem.segments.Count);

            // If the start waypoint is not the last waypoint in the segment, set the start position to the start waypoint and the direction to the next waypoint
            if(waypointList[startWaypointIndex].hasNext())
            {
                startPos = waypointList[startWaypointIndex].position;
                dirPoint = waypointList[startWaypointIndex].nextWaypoint;
            }else if(waypointList[startWaypointIndex].hasPrevious())
            {
                startPos = waypointList[startWaypointIndex].previousWaypoint.transform.position;
                dirPoint = waypointList[startWaypointIndex].currentWaypoint;
            }else{//Should not bee needed
                dirPoint = waypointList[startWaypointIndex].currentWaypoint;
                startPos = waypointList[startWaypointIndex].position-Vector3.forward;
            }
            



            
                Vehicle v=new Vehicle(
                    i,
                    "car" + i,
                    (VehicleType)Random.Range(0, System.Enum.GetValues(typeof(VehicleType)).Length),
                    Random.Range(1, 100), // Example startTime
                    startPos,
                    waypointList[destSegment].position,
                    dirPoint,
                    waypointList[destSegment].segment
                );
            vehicleList.Add(v);
            // DataGatherer.LogEvent(v,LogAction.Start,sim);

            // Optionally, yield to avoid blocking the main thread for large numbers of cars
            if (i % 10 == 0) // Yield every 10 iterations
            {
                yield return null;
            }
        }

        // Mark configuration as ready
        ready = true;

        // Ensure at least one yield
        yield return null;
    }


    public void Start()
    {
        StartCoroutine(VConf());
    }




}
