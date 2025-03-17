using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using UnityEditor.SceneManagement;
using NUnit.Framework.Constraints;
using System.Xml.Serialization;
// using SimConfig;

public class Navigation : MonoBehaviour
{


    [Header("Vehicle Configure")]
    // public GameObject Vehicle;
    public Vehicle_AI Vehicle_AI;
    public StopWatch sw;
    public StopWatch segmentsw;
    // public Description Description;
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
    // vehicle type
    public enum VehicleType
    {
        CAV,
        NonCAV
    }
    public VehicleType vehicleType;


    
    private void Start()
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

        // UpdateCurrentSegment();   
    }

    public void CurSegSet(){
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

    // void Update(){
    //     DistanceToDestination=Vector3.Distance(Vehicle.transform.position, dest);
    //     positionOfCurrentFrame=Vehicle.transform.position;
    //     prevTime=currentTime;
    //     currentTime=sw.getTime();
    //     speed=getSpeed();
    // }

    void Update()
    {
        DistanceToDestination = Vector3.Distance(Vehicle.transform.position, dest);
        positionOfPrevFrame = positionOfCurrentFrame;
        positionOfCurrentFrame = Vehicle.transform.position;
        prevTime = currentTime;
        currentTime = sw.getTime();
        speed = getSpeed();
        communicationAgent.SendMessageToUpdateRoad(ID, CurrentSegment.id, speed, segmentsw.getTime());

        // Detect if the segment has changed
        // int newSegmentID = Vehicle_AI.getCurrentTarget().segment;
        // if (newSegmentID != CurrentSegment.id)
        // {
        //     // TODO:- leave old segment and destroy related information
        //     communicationAgent.SendMessageToRoad("exit", ID, CurrentSegment.id);
        //     UpdateCurrentSegment();
        //     //TODO - Join new segment and update information

        // }
    }
    public void ExitSegment(){
        segmentsw.stopTimer();
        communicationAgent.SendMessageToRoad("exit", ID, CurrentSegment.id);
        segmentsw.resetTimer();
    }
    public void EnterSegment(){
        if(communicationAgent==null){
            communicationAgent = Vehicle.GetComponent<CommunicationAgent>();
        }
        if(segmentsw==null){
            segmentsw = Vehicle.AddComponent<StopWatch>();
        }
        if(CurrentSegment==null){
            CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        }
        communicationAgent.SendMessageToRoad("enter", ID, CurrentSegment.id);
        segmentsw.startTimer();
    }
    public void ExitEnterSegment()
    {
        segmentsw.stopTimer();
        communicationAgent.SendMessageToRoad("exit", ID, CurrentSegment.id);
        communicationAgent.SendMessageToRoad("enter", ID, CurrentSegment.id);
        segmentsw.resetTimer();
        segmentsw.startTimer();
        CurrentSegment=trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        CurrentSegmentCost = CurrentSegment.cost;
    }
    // public void UpdateCurrentSegment()
    // {
    //     // segmentsw.stopTimer();

    //     // CurrentSegment = trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
    //     // // Debug.Log("Current Segment: "+CurrentSegment.id);
    //     // // Debug.Log("ID: "+ID);
    //     // communicationAgent.SendMessageToRoad("enter", ID, CurrentSegment.id);
    //     // //send time spent to segment
    //     // segmentsw.resetTimer();
    //     // segmentsw.startTimer();
    //     // CurrentSegmentCost = CurrentSegment.cost;
    //     // Debug.Log(CurrentSegment.id);
    //     // path.Remove(0);

    //     if (vehicleType == VehicleType.CAV)
    //     {
    //         CurrentSegmentCost = CurrentSegment.dynamicCost;

    //         // dynamicAStar = new DynamicAStar();
    //         // dynamicAStar.AStarPathfinder(trafficSystem.segments);
    //         path = dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
    //         string s = "start segment:" + CurrentSegment.id + "path:" + "\n";
    //         foreach (int i in path)
    //         {
    //             s += i + " ";
    //         }
    //         Debug.LogWarning(s);

    //         path = dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
    //         if (path == null || path.Count == 0)
    //         {
    //             Debug.LogWarning("Dynamic A* failed: No path found!");
    //         }
    //         else
    //         {
    //             Debug.Log($"New path found with {path.Count} segments.");
    //         }

    //         // Debug.Log("Dynamic A* path generated");
    //         // path.Remove(0);
    //         // if (path.Count > 0) path.RemoveAt(0);

    //     }



    // }

    // public void UpdatePath()
    // {
    //     path = staticAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
    // }

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

public int dynamicGenCall(){
        path = dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Dynamic A* failed: No path found!");
                return -1;
            }
            return 0;
}
    public int GetNextSegmentId()
    {
        if(vehicleType == VehicleType.CAV){
            int c=dynamicGenCall();
            if(c==-1){
                return -1;
            }
        }

        // Debug.Log("GetNext Called");
        path.RemoveAt(0);
        if (path.Count == 0)
        {
            // Debug.Log("Path is empty");
            return -1;
        }
        // UpdateCurrentSegment();
        // Debug.Log(path[0]);
        return path[0];


        // if(trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count == 0)
        //     return 0;
        // int c = Random.Range(0, trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count);//segment selection
        // return trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments[c].id;



        // int tbr=path[1];
        //     path.Remove(0);
        //     return tbr;
        // if(path.Count>0){

        //     // Debug.Log(tbr);
        //     return tbr;
        // }else{
        //     Debug.Log("has empty path");

        //     // if(pathGenerated){
        //     // Vehicle.GetComponent<Description>().endOfLife();
        //     // }
        //     return 0;//TODO: implement end of journey
        // }
    }

    public float getSpeed()
    {
        float speed = Vector3.Distance(positionOfPrevFrame, positionOfCurrentFrame) / (currentTime - prevTime);
        return speed;
    }




}