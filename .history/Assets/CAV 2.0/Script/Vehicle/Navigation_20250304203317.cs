using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using UnityEditor.SceneManagement;
// using SimConfig;

public class Navigation : MonoBehaviour
{


    [Header("Vehicle Configure")]
    public GameObject Vehicle;
    public Vehicle_AI Vehicle_AI;
    public Description Description;

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;


    [Header("Communication")]
    public int ID;

    [Header("Navigation")]
    public List<int> path;
    public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
    public Segment CurrentSegment;
    public int CurrentSegmentCost;
    public Segment DestinationSegment;

    StaticAStar staticAStar;
    DynamicAStar dynamicAStar;
    bool pathGenerated=false;

    // vehicle type
    public enum VehicleType
    {
        CAV,
        NonCAV
    }
    public VehicleType vehicleType;

    private void Start()
    {
        // Vehicle_AI=Vehicle.GetComponent<Vehicle_AI>();
        //At start
        UpdateCurrentSegment();
        staticAStar = new StaticAStar();
        dynamicAStar = new DynamicAStar();
        
        staticAStar.AStarPathfinder(trafficSystem.segments);
        dynamicAStar.AStarPathfinder(trafficSystem.segments);

        if(vehicleType==VehicleType.CAV){
            path=dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
        }
        else{
            path=staticAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
        }
        pathGenerated=true;
        

    }

    public void UpdateCurrentSegment(){

        CurrentSegment=trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        CurrentSegmentCost=CurrentSegment.cost;
        if(vehicleType==VehicleType.CAV){
            CurrentSegmentCost=CurrentSegment.dynamicCost;
            if(dynamicAStar==null){
            dynamicAStar = new DynamicAStar();
            dynamicAStar.AStarPathfinder(trafficSystem.segments);
            path=dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
        }

        //drop first segment in path
        path.RemoveAt(0);
        Debug.Log("path regenerated");
        }
        
        //Log update
        
        //Get new path if CAV

    }


    public int GetNextSegmentId(){
                // if(trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count == 0)
                //     return 0;
                // int c = Random.Range(0, trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count);//segment selection
                // return trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments[c].id;

                if(path.Count>0){
                    int tbr=path[0];
                    path.Remove(0);
                    // Debug.Log(tbr);
                    return tbr;
                }else{
                    // if(pathGenerated){
                    // Vehicle.GetComponent<Description>().endOfLife();
                    // }
                    return 0;//TODO: implement end of journey
                }
    }


    

}