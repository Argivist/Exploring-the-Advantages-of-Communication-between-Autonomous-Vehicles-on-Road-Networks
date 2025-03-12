using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using UnityEditor.SceneManagement;
using NUnit.Framework.Constraints;
// using SimConfig;

public class Navigation : MonoBehaviour
{


    [Header("Vehicle Configure")]
    // public GameObject Vehicle;
    public Vehicle_AI Vehicle_AI;
    // public Description Description;
    public GameObject Vehicle;
    public Vector3 dest;

    

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;


    [Header("Communication")]
    public int ID;

    [Header("Navigation")]
    public float DistanceToDestination;
    public float destinationThreshold=1;
    public List<int> path;
    // public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
    public Segment CurrentSegment;
    public int CurrentSegmentCost;
    public Segment DestinationSegment;

    StaticAStar staticAStar;
    DynamicAStar dynamicAStar;
    bool pathGenerated=false;

    // Vehicle share info
    Vector3 positionOfPrevFrame;
    Vector3 positionOfCurrentFrame;
    // vehicle type
    public enum VehicleType
    {
        CAV,
        NonCAV
    }
    public VehicleType vehicleType;

    

    private void Start()
    {
        Vehicle_AI=Vehicle.GetComponent<Vehicle_AI>();

        //At start

        staticAStar = new StaticAStar();
        dynamicAStar = new DynamicAStar();
        
        staticAStar.AStarPathfinder(trafficSystem.segments);
        dynamicAStar.AStarPathfinder(trafficSystem.segments);
        
        UpdateCurrentSegment();

        if(vehicleType==VehicleType.CAV){
            path=dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
            // path.Remove(0);
        }
        else{
            path=staticAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
            // path.RemoveAt(0);
        }
        // pathGenerated=true;
        pathGenerated=true;

    }

    void Update(){
        DistanceToDestination=Vector3.Distance(Vehicle.transform.position, dest);
        positionOfPrevFrame=positionOfCurrentFrame;
        positionOfCurrentFrame=Vehicle.transform.position;
    }

    public void UpdateCurrentSegment(){
        CurrentSegment=trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        CurrentSegmentCost=CurrentSegment.cost;
        Debug.Log(CurrentSegment.id);
        // path.Remove(0);

        if(vehicleType==VehicleType.CAV){
            CurrentSegmentCost=CurrentSegment.dynamicCost;

            // dynamicAStar = new DynamicAStar();
            // dynamicAStar.AStarPathfinder(trafficSystem.segments);
            path=dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
            Debug.Log("Dynamic A* path generated");
            // path.Remove(0);
            // if (path.Count > 0) path.RemoveAt(0);

        }

        // //drop first segment in path
        // // path.RemoveAt(0);
        // Debug.Log("path regenerated");
        // }
        
        //Log update
        
        //Get new path if CAV

    }

    public void UpdatePath(){
        path=staticAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
    }

    public bool destinationReached(Vector3 pos){
        if(Vector3.Distance(pos, dest)<destinationThreshold){
            return true;
        }else{
            return false;
        }
    }

    public int GetNextSegmentId(){
            UpdateCurrentSegment();
            
            // Debug.Log("GetNext Called");
            path.RemoveAt(0);
            Debug.Log(path[0]);
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

    public float getSpeed(){

    }




}