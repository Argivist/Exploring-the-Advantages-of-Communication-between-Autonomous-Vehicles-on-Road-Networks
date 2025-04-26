using System.Collections;
using System.Collections.Generic;
using System.IO;
using TrafficSimulation;
using UnityEngine;
using static Navigation;

[System.Serializable]
public class Vehicle
{
    public int vehicleId;
    public string vehicleName;
    public VehicleType vehicleType;
    public int startTime;
    public Vector3 startPos;
    public Vector3 endPos;
    public Waypoint wdir;
    public Segment startSeg;
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

    public bool HasNext() => nextWaypoint != null;
    public bool HasPrevious() => previousWaypoint != null;
}

///!Json segment
[System.Serializable]
public class Waypoint_js
{
    public float x;
    public float y;

    public Waypoint_js(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class Segment_js
{
    public int segmentId; // Optional ID
    public List<Waypoint_js> waypoints;

    public Segment_js(int id)
    {
        segmentId = id;
        waypoints = new List<Waypoint_js>();
    }
}

[System.Serializable]
public class SegmentNetwork_js
{
    public List<Segment_js> segments = new List<Segment_js>();
}


/////////////////

public class SimulationConfigurer : MonoBehaviour
{
    [Header("Simulation Configurer")]
    public int VehicleDensity;
    public bool ready = false;

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;

    [Header("Vehicle List")]
    public List<Vehicle> vehicleList = new List<Vehicle>();

    [Header("Start Destination type")]
    public bool ManualStartDestinationSetup = false;
    public Segment StartPosition;
    public Segment EndPosition;

    [HideInInspector]
    public DataHandler dataHandler;
    public bool default_ = true;

    IEnumerator VConf()
    {
        // Wapoint List and Segment Network
        List<WaypointObject> waypointList = new List<WaypointObject>();
        SegmentNetwork_js network = new SegmentNetwork_js();

        // Fill the waypointList with all waypoints in the traffic system
        foreach (Segment segment in trafficSystem.segments)
        {
            Segment_js segment_js = new Segment_js(segment.id);
            for (int i = 0; i < segment.waypoints.Count; i++)
            {
                Waypoint waypoint = segment.waypoints[i];
                Waypoint nextWaypoint = (i < segment.waypoints.Count - 1) ? segment.waypoints[i + 1] : null;
                Waypoint previousWaypoint = (i > 0) ? segment.waypoints[i - 1] : null;

                waypointList.Add(new WaypointObject(segment, waypoint, nextWaypoint, previousWaypoint, waypoint.transform.position));
                Waypoint_js waypoint_js = new Waypoint_js(waypoint.transform.position.x, waypoint.transform.position.z);
                segment_js.waypoints.Add(waypoint_js);

            }
            network.segments.Add(segment_js);
        }

        // Creating and filling the vehicle list
        for (int i = 0; i < VehicleDensity; i++)
        {
            List<WaypointObject> validWaypoints = waypointList.FindAll(wp => wp.HasNext());
            if (validWaypoints.Count == 0)
            {
                Debug.LogError("No valid spawn waypoints with a next waypoint found.");
                yield break;
            }
            int startWaypointIndex = Random.Range(0, validWaypoints.Count);
            WaypointObject startWaypointObj = validWaypoints[startWaypointIndex];


            Waypoint startWaypoint = startWaypointObj.currentWaypoint;
            Waypoint dirPoint;
            if (default_)
            {
                dirPoint = startWaypointObj.HasNext() ? startWaypointObj.nextWaypoint : startWaypointObj.previousWaypoint;
            }
            else
            {
                dirPoint = startWaypointObj.HasNext() ? startWaypointObj.nextWaypoint : startWaypointObj.currentWaypoint;
            }
            if (dirPoint == null)
                dirPoint = startWaypoint;

            // Vector3 startPos = Vector3.Lerp(startWaypoint.transform.position, dirPoint.transform.position, Random.Range(0f, 1f));
            Vector3 startPos = Vector3.Lerp(startWaypoint.transform.position, dirPoint.transform.position, Random.Range(0.25f, 0.75f));

            // Select a random destination segment
            Segment endSegment = trafficSystem.segments[Random.Range(0, trafficSystem.segments.Count)];
            if (endSegment.waypoints.Count == 0)
            {
                Debug.LogError("Selected endSegment has no waypoints.");
                continue; // or yield break; depending on desired behavior
            }
            Waypoint endWaypoint = endSegment.waypoints[Random.Range(0, endSegment.waypoints.Count)];
            Vector3 endPos = endWaypoint.transform.position;


            // Waypoint endWaypoint = endSegment.waypoints[Random.Range(0, endSegment.waypoints.Count)];

            // Vector3 endPos = endWaypoint.transform.position;

            // Create and add the vehicle
            Vehicle v = new Vehicle(
                i,
                "car" + i,
                (VehicleType)Random.Range(0, System.Enum.GetValues(typeof(VehicleType)).Length),
                Random.Range(1, 100),
                startPos,
                endPos,
                dirPoint,
                endSegment
            );
            v.startSeg = startWaypointObj.segment;

            vehicleList.Add(v);
            Debug.Log($"Vehicle {i} added at {startPos} to {endPos}");

            dataHandler.AddVehicleData(v.vehicleId, v.startPos, v.endPos, v.vehicleType);

            if (i % 10 == 0)
                yield return null;
        }



        string json = JsonUtility.ToJson(network, true);
        string safeFileName = "network.json";
        if (safeFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            Debug.LogError("Invalid filename detected.");
            yield break;
        }
        string directory = Path.Combine(Application.persistentDataPath, "JSON");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(Path.Combine(directory, safeFileName), json);
        Debug.Log($"Network data saved to {Path.Combine(directory, safeFileName)}");
        ready = true;
        yield return null;
    }

    IEnumerator VConfManual(){
        // Segment Network
        SegmentNetwork_js network = new SegmentNetwork_js();

        // Fill network js
        ///////////////////
        foreach (Segment segment in trafficSystem.segments)
        {
            // Creating a segment json object 
            Segment_js segment_js = new Segment_js(segment.id);
            for (int i = 0; i < segment.waypoints.Count; i++)
            {
                // Adding waypoints to the segment json object
                Waypoint waypoint = segment.waypoints[i];
                Waypoint_js waypoint_js = new Waypoint_js(waypoint.transform.position.x, waypoint.transform.position.z);
                segment_js.waypoints.Add(waypoint_js);
            }
            // Adding the segment json object to the network
            network.segments.Add(segment_js);        
        }
        ////////////////////

        // Vehicle info creator
        for(int i=0; i<VehicleDensity;i++){
            // Defining the start and end positions

            // Start position
            WaypointObject startWaypointObj = new WaypointObject(StartPosition, StartPosition.waypoints[0], StartPosition.waypoints[1], null, StartPosition.waypoints[0].transform.position);
            Waypoint startWaypoint = startWaypointObj.currentWaypoint;
            // Waypoint dirPoint = startWaypointObj.HasNext() ? startWaypointObj.nextWaypoint : startWaypointObj.previousWaypoint;
            Waypoint dorPoint=startWaypointObj.nextWaypoint;
            Vector3 startPos = Vector3.Lerp(startWaypoint.transform.position, dorPoint.transform.position, Random.Range(0.25f, 0.75f));


            //Destination
            


        }





        yield return null;
    }

    void Start()
    {
        StartCoroutine(VConf());
    }
}

