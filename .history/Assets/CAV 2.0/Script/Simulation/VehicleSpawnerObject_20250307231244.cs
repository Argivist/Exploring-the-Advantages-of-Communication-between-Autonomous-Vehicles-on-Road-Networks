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
    public Timer t;
    public int id;
    public VehicleType type;

    public Segment startSegment;
    public Segment endSegment;

    [Header("Other")]
    Collider c;
    public float spawnInterval;//may not be necessary
    // Start is called before the first frame update
    void Start()
    {
        c=GetComponent<Collider>();
        c.isTrigger=true;    
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
                            float z=w.transform.position.z-this.transform.position.z;
                            float x=w.transform.position.x-this.transform.position.x;
                            float angle=Mathf.Atan2(z , x)*Mathf.Rad2Deg;
                            this.transform.rotation=Quaternion.Euler(0,-angle,0);
                        }
                    }
                    
    }

    // Update is called once per frame
    void Update()
    {
        if(t.isTime()&&){
            spawnVehicle();
        }

        //if no vehicle in me and its time
        //spawn and configure vehicle

        //destroy self
    }
}
