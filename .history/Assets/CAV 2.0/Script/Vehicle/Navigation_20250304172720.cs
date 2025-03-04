using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class Navigation : MonoBehaviour
{
    [Header("Vehicle Configure")]
    public Vehicle_AI Vehicle_AI;

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;


    [Header("Communication")]
    public int ID;

    [Header("NAvigation")]
    public List<int> path;
    public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
    public Segment CurrentSegment;
    public int CurrentSegmentCost;
    public Segment DestinationSegment;
    // public List<Segment> Map;
    // public SimConfig.VehicleType vehicleType;
    // public List<Segment> path;

    // public Vector3 currentPos;
    // public Segment currentSegment;
    // public Segment nextSegment;

    // public Vehicle_AI thisAI;

    // public CarAI carAI;

    private void Start()
    {
        //At start
        CurrentSegment=trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment];
        Debug.Log("Start ID:"+StartSegment.cost+" cost: "+StartSegment.cost);
        Debug.Log("Destination ID:"+DestinationSegment.cost+" cost: "+DestinationSegment.cost);
        


        // // Initialize components and variables
        // thisAI = GetComponent<Vehicle_AI>();
        // comagent = GetComponent<CommunicationAgent>();

        // // Get current position and segment
        // currentPos = transform.position;
        // currentSegment = comagent.getCurrentSegment(currentPos);

        // if (currentSegment == null)
        // {
        //     Debug.LogError("Failed to determine the current segment.");
        //     return;
        // }

        // Debug.Log($"Current segment: {currentSegment.id}");
        // comagent.EnterSegment(currentSegment);

        // // Generate initial path
        // path = RandomPath(currentSegment);

        // if (path.Count > 1)
        // {
        //     nextSegment = path[1];
        //     path.RemoveAt(0); // Remove the current segment from the path
        // }
        // else
        // {
        //     Debug.LogError("Initial path generation failed or has insufficient segments.");
        // }

        // // thisAI.currentSegment = currentSegment;
        // carAI = GetComponent<CarAI>();
        // carAI.SetPath(vectorpath(currentSegment));
        // Debug.Log("vectorpath: " + vectorpath(currentSegment));

    }

    public void UpdateCurrentSegment(){

    }


    public int GetNextSegmentId(){
                if(trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count == 0)
                    return 0;
                int c = Random.Range(0, trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments.Count);//segment selection
                return trafficSystem.segments[Vehicle_AI.getCurrentTarget().segment].nextSegments[c].id;
    }


    //!SECTION Navigation Algorithm
    //ANCHOR - Static A*
    void static_A_starNav(){

    }
    //ANCHOR - Dynamic A*!SECTION
    void A_star_dynamic(){

    }

}
    // public List<Vector3> vectorpath(Segment s)
    // {
    //     List<Vector3> path = new List<Vector3>();
    //     s.waypoints.ForEach(wp => path.Add(wp.transform.position));
    //     return path;
    // }
    // private void Update()
    // {
    //     if (vehicleType == SimConfig.VehicleType.CAV && path.Count <= 1)
    //     {
    //         // Regenerate path for CAV vehicles if near the end of the current path
    //         // path = RandomPath(currentSegment);

    //         if (path.Count > 1)
    //         {
    //             nextSegment = path[1];
    //             path.RemoveAt(0);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Path regeneration failed for CAV.");
    //         }
    //     }
    // }

    // public bool OntoNextSegment()
    // {
    //     // Move to the next segment
    //     if (path == null || path.Count < 1)
    //     {
    //         Debug.LogError("Path is empty. Cannot move to the next segment.");
    //         //TODO - Send some end of journey message
    //         return true;
    //     }
    //     else
    //     {
    //         Debug.Log("Path is not empty");

    //     }

    //     comagent.ExitSegment(currentSegment);
    //     comagent.EnterSegment(nextSegment);

    //     currentSegment = nextSegment;

    //     if (path.Count > 1)
    //     {
    //         nextSegment = path[1];
    //         path.RemoveAt(0);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Path is about to be exhausted.");
    //         // path = RandomPath(currentSegment); // Regenerate path
    //         if (path.Count > 0)
    //             nextSegment = path[0];
    //     }

    //     // thisAI.currentSegment = currentSegment;
    //     carAI.SetPath(vectorpath(currentSegment));
    //     return false;
    // }

    // Random path generator


    // public bool OntoNextSegment()
    // {
    //     // Check if the path is empty or has no further segments
    //     if (path == null || path.Count < 1)
    //     {

    //         comagent.ExitSegment(currentSegment);
    //         Debug.Log("End of journey reached. Returning true.");
    //         // Handle end-of-journey behavior if necessary
    //         carAI.StopVehicle(); // Example: stop the vehicle
    //         return true; // Indicate the journey is complete
    //     }

    //     Debug.Log("Path is not empty. Moving to the next segment.");

    //     // Transition to the next segment
    //     comagent.ExitSegment(currentSegment);
    //     comagent.EnterSegment(nextSegment);

    //     currentSegment = nextSegment;

    //     if (path.Count > 1)
    //     {
    //         nextSegment = path[1];
    //         path.RemoveAt(0);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Path is about to be exhausted.");

    //         // Check if path exhaustion means end of journey
    //         if (vehicleType == SimConfig.VehicleType.CAV)
    //         {
    //             Debug.Log("Generating new path for CAV.");
    //             path = RandomPath(currentSegment);

    //             if (path.Count > 0)
    //             {
    //                 nextSegment = path[0];
    //                 path.RemoveAt(0);
    //             }
    //             else
    //             {
    //                 Debug.Log("No new path generated. End of journey reached.");
    //                 carAI.StopVehicle(); // Example stop logic
    //                 return true; // Indicate the journey is complete
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("End of journey reached for non-CAV vehicle.");
    //             carAI.StopVehicle(); // Example stop logic
    //             return true; // Indicate the journey is complete
    //         }
    //     }

    //     // Update AI with the new segment path
    //     carAI.SetPath(vectorpath(currentSegment));
    //     return false; // Indicate the journey is not yet complete
    // }

    // //!SECTION: Path Finder
    // public List<Segment> RandomPath(Segment currentSegment)
    // {
    //     if (currentSegment == null || currentSegment.nextSegments == null || currentSegment.nextSegments.Count == 0)
    //     {
    //         Debug.LogError("Current segment is null or has no connected segments.");
    //         return new List<Segment>();
    //     }

    //     List<Segment> newPath = new List<Segment> { currentSegment };
    //     Segment tempSegment = currentSegment;

    //     for (int i = 0; i < 5; i++)
    //     {
    //         if (tempSegment.nextSegments == null || tempSegment.nextSegments.Count == 0)
    //         {
    //             Debug.LogWarning($"Segment {tempSegment.id} has no further connections. Path generation stopped.");
    //             break;
    //         }

    //         tempSegment = tempSegment.nextSegments[Random.Range(0, tempSegment.nextSegments.Count)];
    //         newPath.Add(tempSegment);
    //     }

    //     return newPath;
    // }

    // public List<Segment> CAVNav(Segment start, Segment end)
    // {
    //     //called on end of current segment
    //     return null;
    // }

    // public List<Segment> OneNav(Segment start, Segment end)
    // {
    //     // called once replace random path
    //     return null;
    // }

    // public Segment nextSegments()
    // {
    //     //get the next segment
    //     currentSegment = nextSegment;
    //     //get the next segment
    //     nextSegment = path[path.IndexOf(currentSegment) + 1];
    //     return currentSegment;
    // }

    

