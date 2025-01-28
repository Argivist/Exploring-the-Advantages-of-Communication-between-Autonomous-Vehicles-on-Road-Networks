using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class WayPointNavigator : MonoBehaviour
{
    // Start is called before the first frame update

    CarController controller;
    CarAI carAI;
    public Waypoint CurrentWaypoint;

    private void Awake()
    {
        controller = GetComponent<CarController>();
        carAI = GetComponent<CarAI>();
        GameObject car;

    
    } 
    void Start()
    {
        
        //get the first waypoint
        CurrentWaypoint = CurrentWaypoint.NextWayPoint;
        //get the destination waypoint any random waypoint
        int rand = Random.Range(0, 10);
        for (int i = 0; i < rand; i++)
        {
            CurrentWaypoint = CurrentWaypoint.NextWayPoint;
        }

        //get the path to the waypoint
        List<Vector3> path = new List<Vector3>();
        while (CurrentWaypoint != null)
        {
            path.Add(CurrentWaypoint.GetPosition());
            CurrentWaypoint = CurrentWaypoint.NextWayPoint;
        }

        //set the CarAI path
        carAI.SetPath(path);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
