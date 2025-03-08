using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using static Navigation;

public class VehicleSpawnerObject : MonoBehaviour
{

    [Header("Vehicle Object")]
    public GameObject vehicleObject;


    [Header("SpawnInfo")]
    public int id;
    public VehicleType type;

    public Segment startSegment;
    public Segment endSegment;

    [Header("Other")]
    public float spawnInterval;//may not be necessary
    // Start is called before the first frame update
    void Start()
    {
        //get near waypoint in start segment
        
    }



    void direction(){
        //Find nearest waypoint to start within the segment
                    float minDist = float.MaxValue;
                    for(int j=0; j<startSegment.waypoints.Count; j++){
                        float d = Vector3.Distance(this.transform.position, startSegment.waypoints[j].transform.position);

                        //Only take in front points
                        Vector3 lSpace = this.transform.InverseTransformPoint(startSegment.waypoints[j].transform.position);
                        if(d < minDist && lSpace.z > 0){
                            minDist = d;
                            currentTarget.waypoint = j;
                        }
                    }
                    break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
