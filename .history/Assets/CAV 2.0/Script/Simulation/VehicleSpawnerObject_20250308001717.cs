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
    Quaternion startRotation;
    float angle;

    [Header("Other")]
    Collider c;
    bool canSpawn=false;
    public float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        c=GetComponent<Collider>();
        c.isTrigger=true;    
        direction();
        
    }

void direction()
{
    float minDist = float.MaxValue;
    for (int j = 0; j < startSegment.waypoints.Count; j++)
    {
        float d = Vector3.Distance(transform.position, startSegment.waypoints[j].transform.position);
        Vector3 lSpace = transform.InverseTransformPoint(startSegment.waypoints[j].transform.position);
        if (d < minDist && lSpace.z > 0)
        {
            minDist = d;
            Waypoint w = startSegment.waypoints[j];
            float z = w.transform.position.z - transform.position.z;
            float x = w.transform.position.x - transform.position.x;
            angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
            startRotation = Quaternion.Euler(0, angle+90, 0);
            this.transform.rotation = startRotation;

            Debug.Log("Calculated Angle: " + angle);
            Debug.Log("Start Rotation: " + startRotation.eulerAngles);
        }
    }
    canSpawn = true;
}

void Update()
{
    if (canSpawn)
    {
        GameObject vClone = Instantiate(vehicleObject, transform.position, transform.rotation);
        // Debug.Log("Instantiated Rotation: " + vClone.transform.rotation.eulerAngles);
        Description d = vClone.GetComponent<Description>();
        Navigation n = vClone.GetComponent<Navigation>();
        CommunicationAgent cm = vClone.GetComponent<CommunicationAgent>();
        d.id = id;
        n.vehicleType = type;
        n.CurrentSegment = startSegment;
        n.DestinationSegment = endSegment;
        cm.id = id;
        canSpawn = false;
    }
}
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="AutonomousVehicle"){
            canSpawn=false;
        }else{
            canSpawn=true;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="AutonomousVehicle"){
            canSpawn=true;
        }else{
            canSpawn=false;
        }

    }
    void OnTriggerStay(Collider other){
        if(other.gameObject.tag=="AutonomousVehicle"){
            canSpawn=false;
        }else{
            canSpawn=true;
        }
    }
}
