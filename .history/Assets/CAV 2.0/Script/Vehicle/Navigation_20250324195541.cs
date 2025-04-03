using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using UnityEditor.SceneManagement;
using NUnit.Framework.Constraints;
using System.Xml.Serialization;

public class Navigation : MonoBehaviour
{

    [Header("Vehicle Configure")]
    public Vehicle_AI Vehicle_AI;
    public StopWatch sw;
    public StopWatch segmentsw;
    public GameObject Vehicle;
    public Vector3 dest;
    public float speed;
    public CommunicationAgent communicationAgent;


    [Header("Traffic System")]
    public TrafficSystem trafficSystem;


    [Header("Communication")]
    public int ID;

    [Header("Navigation")]
    public float DistanceToDestination;
    public float destinationThreshold = 1;
    public List<int> path;
    // public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
    public Segment CurrentSegment;
    public float CurrentSegmentCost;
    public Segment DestinationSegment;


    StaticAStar staticAStar;
    DynamicAStar dynamicAStar;

    // Vehicle share info
    Vector3 positionOfPrevFrame;
    Vector3 positionOfCurrentFrame;

    int prevTime;
    int currentTime;

    int LastSegment;
    bool last_road;
    // vehicle type
    public enum VehicleType
    {
        CAV,
        NonCAV
    }
    public VehicleType vehicleType;

    public bool isReady = false;
    void Start()
    {
        // Component initialization
        communicationAgent = Vehicle.GetComponent<CommunicationAgent>();
        positionOfPrevFrame = Vehicle.transform.position;
        positionOfCurrentFrame = Vehicle.transform.position;
        Vehicle_AI = Vehicle.GetComponent<Vehicle_AI>();
        sw = Vehicle.GetComponent<StopWatch>();
        segmentsw = Vehicle.AddComponent<StopWatch>();



        // Navigation initialization
        staticAStar = new StaticAStar();
        dynamicAStar = new DynamicAStar();


        sw.startTimer();
        segmentsw.startTimer();
        prevTime = sw.getTime();
        currentTime = sw.getTime();


        staticAStar.AStarPathfinder(trafficSystem.segments);
        dynamicAStar.AStarPathfinder(trafficSystem.segments);

        isReady = true;
    }


    void Update()
    {
        DistanceToDestination = Vector3.Distance(Vehicle.transform.position, dest);
        positionOfPrevFrame = positionOfCurrentFrame;
        positionOfCurrentFrame = Vehicle.transform.position;
        prevTime = currentTime;
        currentTime = sw.getTime();
        speed = getSpeed();
        communicationAgent.SendMessageToUpdateRoad(ID, CurrentSegment.id, speed, segmentsw.getTime());
        if(path==null){
            Debug.LogWarning("Path is null");
        }
        //if path is empty, destination is behind the vehicle destroy
        if (path.Count == 0)
        {
            last_road = true;
        }
        //if last road is true and the last segment has changed, call neat destroy_
        if (last_road && Vehicle_AI.VIsOnSegment(gameObject.transform.position, LastSegment))
        {
            Vehicle_AI.DestroyVehicle_();
        }
    }

    public void ExitSegment()
    {
        if (communicationAgent == null)
        {
            communicationAgent = Vehicle.GetComponent<CommunicationAgent>();
        }
        if (segmentsw == null)
        {
            segmentsw = Vehicle.AddComponent<StopWatch>();
        }
        if (CurrentSegment == null)
        {
            CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        }
        segmentsw.stopTimer();
        communicationAgent.SendMessageToRoad("exit", ID, CurrentSegment.id);
        segmentsw.resetTimer();
    }

    public void EnterSegment()
    {
        if (communicationAgent == null)
        {
            communicationAgent = Vehicle.GetComponent<CommunicationAgent>();
        }
        if (segmentsw == null)
        {
            segmentsw = Vehicle.AddComponent<StopWatch>();
        }
        if (CurrentSegment == null)
        {
            CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        }
        communicationAgent.SendMessageToRoad("enter", ID, CurrentSegment.id);
        segmentsw.startTimer();
    }
    public void CurSegSet()
    {
        staticAStar = new StaticAStar();
        dynamicAStar = new DynamicAStar();
        staticAStar.AStarPathfinder(trafficSystem.segments);
        dynamicAStar.AStarPathfinder(trafficSystem.segments);
        CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        // Debug.LogWarning("Current Segment: " + CurrentSegment.id);

        positionOfPrevFrame = positionOfCurrentFrame;
        if (vehicleType == VehicleType.CAV)
        {
            path = dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);

        }
        else
        {
            path = staticAStar.FindPath(CurrentSegment.id, DestinationSegment.id);

        }
    }
    public bool destinationReached(Vector3 pos)
    {

        if (Vector3.Distance(pos, dest) < destinationThreshold)
        {
            //print vehice numbeer and destination reached
            Debug.Log("Vehicle " + ID + " reached destination");
            return true;
        }
        else
        {

            return false;
        }
    }
    public int dynamicGenCall()
    {
        if (dynamicAStar == null)
        {
            dynamicAStar = new DynamicAStar();
            dynamicAStar.AStarPathfinder(trafficSystem.segments);
        }
        if (CurrentSegment == null)
        {
            CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
            // CurrentSegment = trafficSystem.segments[Vehicle_AI.getNextTarget().segment];
        }
        // path = dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
        path = dynamicAStar.FindPath(path[0], DestinationSegment.id);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Dynamic A* failed: No path found!");
            return -1;
        }
        return 0;
    }

    public int GetNextSegmentId()
    {
        if (vehicleType == VehicleType.CAV)
        {
            // string s = "";
            // foreach (int i in path)
            // {
            //     s += i + " ";
            // }
            // Debug.LogWarning("1-Vehicle " + ID + " path: " + s +"current segment: "+CurrentSegment.id+" destination segment: "+DestinationSegment.id);
            // Debug.LogWarning("Vehicle " + ID + " path: " + path.Count);	

            int c = dynamicGenCall();
            // path.RemoveAt(0);  sometimes it adds past to the  path for cav

            // s = "";
            // foreach (int i in path)
            // {
            //     s += i + " ";
            // }
            // Debug.LogWarning("2-Vehicle " + ID + " path: " + s+"current segment: "+CurrentSegment.id+" destination segment: "+DestinationSegment.id);
            if (c == -1)
            {
                Debug.LogWarning("Dynamic A* failed: No path found!");
                return -1;
            }
            // s = "";
            // foreach (int i in path)
            // {
            //     s += i + " ";
            // }
            // Debug.LogWarning("3-Vehicle " + ID + " path: " + s+"current segment: "+CurrentSegment.id+" destination segment: "+DestinationSegment.id);
        }

        path.RemoveAt(0);
        if (path.Count == 0)
        {
            Debug.LogWarning("** Vehicle " + ID + " path: " + path.Count);
            return -1;
        }

        return path[0];


    }

    public float getSpeed()
    {
        float speed = Vector3.Distance(positionOfPrevFrame, positionOfCurrentFrame) / (currentTime - prevTime);
        return speed;
    }
}