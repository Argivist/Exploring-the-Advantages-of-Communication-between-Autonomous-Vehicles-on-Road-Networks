using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using UnityEngine.U2D;
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
        direction();
        
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
                            // currentTarget.waypoint = j;//index of the waypoint
                            //get waypoint direction
                            Waypoint w = startSegment.waypoints[j];
// Both positions
                            Debug.Log("Start: "+this.transform.position);
                            Debug.Log("Waypoint: "+w.transform.position);

                            float angle=Vector3.Angle(this.transform.position,w.transform.position);
                            Debug.Log("Angle: "+andle);

                            // get direction of waypoint
                            // Vector3 dir=(w.transform.position - this.transform.position).normalized;
                            // dir.y=0;
                            // Vector3 F=transform.forward;
                            // float theta=Vector3.SignedAngle(F,dir,Vector3.up);

                            // Debug.Log("Theta: "+theta);
                        
                        }
                    }
                    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
