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

    // Update is called once per frame
    void Update()
    {
        
    }
}
