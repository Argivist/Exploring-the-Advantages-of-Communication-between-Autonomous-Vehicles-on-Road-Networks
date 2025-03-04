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
            // path.Remove(0);
        }
        // pathGenerated=true;
        

    }

    public void UpdateCurrentSegment(){
        CurrentSegment=trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        CurrentSegmentCost=CurrentSegment.cost;
        Debug.Log(CurrentSegment);
        // path.Remove(0);
        // if(vehicleType==VehicleType.CAV){
        //     CurrentSegmentCost=CurrentSegment.dynamicCost;
        //     if(dynamicAStar==null){
        //     dynamicAStar = new DynamicAStar();
        //     dynamicAStar.AStarPathfinder(trafficSystem.segments);
        //     path=dynamicAStar.FindPath(CurrentSegment.id, DestinationSegment.id);
        //     path.Remove(0);
        // }

        // //drop first segment in path
        // // path.RemoveAt(0);
        // Debug.Log("path regenerated");
        // }
        
        //Log update
        
        //Get new path if CAV

    }


    public int GetNextSegmentId(){
            UpdateCurrentSegment();
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


    

}