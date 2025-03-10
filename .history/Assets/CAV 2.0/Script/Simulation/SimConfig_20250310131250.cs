using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using static Navigation;

public class SimConfig : MonoBehaviour
{


    //personal variables
    public int num_cars;
    public bool ready = false;

    //Existing objects
    public TrafficSystem trafficSystem;

    //Custom Data Structures
    // Vehicle Types

    //Loggable actions
    public enum LogAction
    {
        Start,
        Stop,
        Enter,
        Exit
        // Accelerate,
        // Decelerate,
        // Turn,
        // LaneChange
    }



    ///Custom objects
    // Vehicle info object
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

        public Vehicle(int vehicle_id, string vehicleName, VehicleType vehicleType, int startTime, Vector3 startPos, Vector3 endPos,Waypoint wdir)
        {
            this.vehicleId = vehicle_id;
            this.vehicleName = vehicleName;
            this.vehicleType = vehicleType;
            this.startTime = startTime;
            this.startPos = startPos;
            this.endPos = endPos;
            this.wdir = wdir;
        }
    }
    //Vehicle Log Object
    [System.Serializable]
    public class VehicleLog
    {
        public string vehicleName;
        public int simulation;
        public int logTime;
        public Vector3 logPosition;
        public Segment logSegment;
        public LogAction logAction;
        public float logSpeed;
        public float logAcceleration;

        public VehicleLog(string vehicleName, int simulation, int logTime, Vector3 logPosition, Segment logSegment, LogAction logAction, float logSpeed, float logAcceleration)
        {
            this.vehicleName = vehicleName;
            this.simulation = simulation;
            this.logTime = logTime;
            this.logPosition = logPosition;
            this.logSegment = logSegment;
            this.logAction = logAction;
            this.logSpeed = logSpeed;
            this.logAcceleration = logAcceleration;
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

    }




    // Vehcile and Log List
    public List<Vehicle> vehicleList = new List<Vehicle>();
    public List<VehicleLog> vehicleLogList = new List<VehicleLog>();

    //configure vehicle objects
    // Configure vehicle objects
    IEnumerator VConf()
    {
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
        for (int i = 0; i < num_cars; i++)
        {
            vehicleList.Add(
                new Vehicle(
                    i,
                    "car" + i,
                    (VehicleType)Random.Range(0, System.Enum.GetValues(typeof(VehicleType)).Length),
                    Random.Range(1, 100), // Example startTime
                    waypointList[Random.Range(0, waypointList.Count)].position,
                    waypointList[Random.Range(0, waypointList.Count)].position
                )
            );

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


    //Log actions
    public void LogStart(){
//log the start of the sim;
    }
    public void LogStop(){
//Log a stop
    }
    public void LogEnter(){
        //log an entry into the road segment
    }
    public void LogExit(){
        //log an exit from a segment
    }


}