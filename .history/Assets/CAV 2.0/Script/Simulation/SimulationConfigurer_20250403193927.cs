using System.Collections;
using System.Collections.Generic;
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
    public bool default_=true;

    IEnumerator VConf()
    {
        List<WaypointObject> waypointList = new List<WaypointObject>();

        foreach (Segment segment in trafficSystem.segments)
        {
            for (int i = 0; i < segment.waypoints.Count; i++)
            {
                Waypoint waypoint = segment.waypoints[i];
                Waypoint nextWaypoint = (i < segment.waypoints.Count - 1) ? segment.waypoints[i + 1] : null;
                Waypoint previousWaypoint = (i > 0) ? segment.waypoints[i - 1] : null;

                waypointList.Add(new WaypointObject(segment, waypoint, nextWaypoint, previousWaypoint, waypoint.transform.position));
            }
        }

        for (int i = 0; i < VehicleDensity; i++)
        {
            int startWaypointIndex = Random.Range(0, waypointList.Count);
            // int startWaypointIndex = Random.Range(1, waypointList.Count);
            WaypointObject startWaypointObj = waypointList[startWaypointIndex];

            Waypoint startWaypoint = startWaypointObj.currentWaypoint;
            Waypoint dirPoint;
            if(default_){
            dirPoint = startWaypointObj.HasNext() ? startWaypointObj.nextWaypoint : startWaypointObj.previousWaypoint;
            }else{
            dirPoint = startWaypointObj.HasNext() ? startWaypointObj.nextWaypoint : startWaypointObj.currentWaypoint;
            }
            if (dirPoint == null)
                dirPoint = startWaypoint;

            // Vector3 startPos = Vector3.Lerp(startWaypoint.transform.position, dirPoint.transform.position, Random.Range(0f, 1f));
            Vector3 startPos = Vector3.Lerp(startWaypoint.transform.position, dirPoint.transform.position, Random.Range(0.25f, 0.75f));

            // Select a random destination segment
            Segment endSegment = trafficSystem.segments[Random.Range(0, trafficSystem.segments.Count)];
            Waypoint endWaypoint = endSegment.waypoints[Random.Range(0, endSegment.waypoints.Count)];

            Vector3 endPos = endWaypoint.transform.position;

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

        ready = true;
        yield return null;
    }

    void Start()
    {
        StartCoroutine(VConf());
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using TrafficSimulation;
// using UnityEngine;
// using static Navigation;
// [System.Serializable]
// public class Vehicle
// {
//     //vehicle name, typ, start time, end time list depending on number o simulations
//     public int vehicleId;
//     public string vehicleName;


//     public VehicleType vehicleType;
//     public int startTime;
//     public Vector3 startPos;
//     public Vector3 endPos;
//     public Waypoint wdir;
//     public Segment destSegment;

//     public Vehicle(int vehicle_id, string vehicleName, VehicleType vehicleType, int startTime, Vector3 startPos, Vector3 endPos, Waypoint wdir, Segment destSegment)
//     {
//         this.vehicleId = vehicle_id;
//         this.vehicleName = vehicleName;
//         this.vehicleType = vehicleType;
//         this.startTime = startTime;
//         this.startPos = startPos;
//         this.endPos = endPos;
//         this.wdir = wdir;
//         this.destSegment = destSegment;

//     }
// }

// [System.Serializable]
// //waypoint with segment, waypoint adn waypoint position
// public class WaypointObject
// {
//     public Segment segment;
//     public Waypoint currentWaypoint;
//     public Waypoint nextWaypoint;
//     public Waypoint previousWaypoint;
//     public Vector3 position;

//     public WaypointObject(Segment segment, Waypoint current, Waypoint next, Waypoint previous, Vector3 position)
//     {
//         this.segment = segment;
//         this.currentWaypoint = current;
//         this.nextWaypoint = next;
//         this.previousWaypoint = previous;
//         this.position = position;
//     }
//     public bool hasNext()
//     {
//         return nextWaypoint != null;
//     }
//     public bool hasPrevious()
//     {
//         return previousWaypoint != null;
//     }

// }


// public class SimulationConfigurer : MonoBehaviour
// {
//     [Header("Simulation Configurer")]

//     public int VehicleDensity;
//     public bool ready = false;

//     [Header("Traffic System")]
//     public TrafficSystem trafficSystem;

//     [Header("Vehicle List")]
//     public List<Vehicle> vehicleList = new List<Vehicle>();

//     [HideInInspector]
//     public DataHandler dataHandler;



//     IEnumerator VConf()
//     {
        
//         // dataHandler = GetComponent<DataHandler>();

//         // Get all available waypoints
//         List<WaypointObject> waypointList = new List<WaypointObject>();
//         foreach (Segment segment in trafficSystem.segments)
//         {
//             for (int i = 0; i < segment.waypoints.Count; i++)
//             {
//                 Waypoint waypoint = segment.waypoints[i];
//                 Waypoint nextWaypoint = i < segment.waypoints.Count - 1 ? segment.waypoints[i + 1] : null;
//                 Waypoint previousWaypoint = i > 0 ? segment.waypoints[i - 1] : null;
//                 waypointList.Add(new WaypointObject(segment, waypoint, nextWaypoint, previousWaypoint, waypoint.transform.position));
//             }
//         }

//         // Configure the cars
//         for (int i = 0; i < VehicleDensity; i++)
//         {
            
//             // Randomly select a start waypoint
//             int startWaypointIndex = Random.Range(0, waypointList.Count);
//             Vector3 startPos = Vector3.zero;
//             Waypoint dirPoint;
//             Segment startSegment = waypointList[startWaypointIndex].segment;
//             int destSegment = Random.Range(0, trafficSystem.segments.Count);
//             Segment endSegment = trafficSystem.segments[destSegment];
//             float x=Random.Range(endSegment.waypoints[0].transform.position.x, endSegment.waypoints[endSegment.waypoints.Count-1].transform.position.x);
//             float z=Random.Range(endSegment.waypoints[0].transform.position.z, endSegment.waypoints[endSegment.waypoints.Count-1].transform.position.z);
//             Vector3 endPos = new Vector3(x, endSegment.waypoints[0].transform.position.y, z);


//             // If the start waypoint is not the last waypoint in the segment, set the start position to the start waypoint and the direction to the next waypoint
//             if (waypointList[startWaypointIndex].hasNext())
//             {
//                 Waypoint startPoint;
//                 // startPos = waypointList[startWaypointIndex].position;
//                 startPoint = waypointList[startWaypointIndex].currentWaypoint;
//                 dirPoint = waypointList[startWaypointIndex].nextWaypoint;
//                 // random position btn start and end
//                 float x_=Random.Range(startPoint.transform.position.x, dirPoint.transform.position.x);
//                 float z_=Random.Range(startPoint.transform.position.z, dirPoint.transform.position.z);
//                 startPos = new Vector3(x_, startPoint.transform.position.y, z_);
//             }
//             else if (waypointList[startWaypointIndex].hasPrevious())
//             {
//                 startPos = waypointList[startWaypointIndex].previousWaypoint.transform.position;
//                 Waypoint startPoint = waypointList[startWaypointIndex].previousWaypoint;
//                 dirPoint = waypointList[startWaypointIndex].currentWaypoint;
//                 float _x=Random.Range(startPoint.transform.position.x, dirPoint.transform.position.x);
//                 float _z=Random.Range(startPoint.transform.position.z, dirPoint.transform.position.z);
//                 startPos = new Vector3(_x, startPoint.transform.position.y, _z);
//             }
//             else
//             {
//                 dirPoint = waypointList[startWaypointIndex].currentWaypoint;
//                 startPos = waypointList[startWaypointIndex].position - Vector3.forward;
//             }

//             // Create the vehicle
//             Vehicle v = new Vehicle(
//                     i,
//                     "car" + i,
//                     (VehicleType)Random.Range(0, System.Enum.GetValues(typeof(VehicleType)).Length),
//                     Random.Range(1, 100), // Example startTime
//                     startPos,
//                     endPos,
//                     // waypointList[destSegment].position
//                     dirPoint,
//                     waypointList[destSegment].segment
//                 );
//             vehicleList.Add(v);

//             Debug.Log("Vehicle added");


//             // Add vehicle data to the data handler
//             dataHandler.AddVehicleData(v.vehicleId, v.startPos, v.endPos, v.vehicleType);



            

//             // Optionally, yield to avoid blocking the main thread for large numbers of cars
//             if (i % 10 == 0) // Yield every 10 iterations
//             {
//                 yield return null;
//             }
//         }

//         // Mark configuration as ready
//         ready = true;

//         // Ensure at least one yield
//         yield return null;
//     }


//     public void Start()
//     {
//         StartCoroutine(VConf());
//     }




// }
