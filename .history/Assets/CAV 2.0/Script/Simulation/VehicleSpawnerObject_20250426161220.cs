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
    public Vector3 dest;
    public Segment destSegment;

    [Header("SpawnInfo")]
    public Timer t;
    public int id;
    public VehicleType type;

    public Segment startSegment;
    public Segment endSegment;

    public Waypoint WayDir;
    Quaternion startRotation;
    float angle;

    [Header("Other")]
    Collider c;
    public DataHandler dataHandler;
    public float spawnTime = 2f; // Time delay between spawns
    public int simNo;

    void Start()
    {
        c = GetComponent<Collider>();
        c.isTrigger = true;
        direction();

        // Start coroutine for spawning vehicles
        StartCoroutine(SpawnVehicles());

        c = GetComponent<Collider>();
    c.isTrigger = true;
    direction();

    // Try to spawn a vehicle immediately
    if (CanSpawn())
    {
        SpawnVehicle();
    }

    // Destroy the spawner immediately after attempting spawn
    Destroy(gameObject);
        
    }

    void direction()
    {
        if (WayDir == null)
    {
        Debug.LogWarning("Waypoint direction is null! Make sure WayDir is assigned.");
        return;
    }
        Waypoint w = WayDir;   
        float z = w.transform.position.z - transform.position.z;
        float x = w.transform.position.x - transform.position.x;
        angle = Mathf.Atan2(x,z) * Mathf.Rad2Deg;
        startRotation = Quaternion.Euler(0, angle, 0);
        this.transform.rotation = startRotation;

    }

    IEnumerator SpawnVehicles()
    {
        while (true) // Infinite loop to keep spawning
        {
            if (CanSpawn())
            {
                GameObject vClone = Instantiate(vehicleObject, transform.position, this.transform.rotation);
                vClone.name = "Vehicle " + id;
                vClone.transform.rotation = startRotation;

                // Assign vehicle properties
                Description d = vClone.GetComponent<Description>();
                CommunicationAgent cm = vClone.GetComponent<CommunicationAgent>();
                Navigation n = vClone.GetComponent<Navigation>();

                d.id = id;
                n.ID = id;
                n.vehicleType = type;
                n.DestinationSegment = endSegment;
                n.dest = dest;
                n.DestinationSegment = destSegment;
                n.segcycle = simNo;
                cm.id = id;
                n.CurrentSegment = startSegment;
                n.dataHandler = dataHandler;
                vClone.SetActive(true);
            }
            Destroy(gameObject);

            yield return new WaitForSeconds(spawnTime); // Wait before next spawn
        }
    }

    bool CanSpawn()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f); // Check for nearby objects
        foreach (Collider col in colliders)
        {
            if (col.gameObject.tag == "AutonomousVehicle")
            {
                return false; // Don't spawn if another vehicle is in range
            }
        }
        return true;
    }
}







//using System.Collections;
// using System.Collections.Generic;
// using TrafficSimulation;
// using UnityEngine;
// using UnityEngine.U2D;
// using static Navigation;

// public class VehicleSpawnerObject : MonoBehaviour
// {
//     [Header("Vehicle Object")]
//     public GameObject vehicleObject;
//     public Vector3 dest;
//     public Segment destSegment;

//     [Header("SpawnInfo")]
//     public Timer t;
//     public int id;
//     public VehicleType type;

//     public Segment startSegment;
//     public Segment endSegment;

//     public Waypoint WayDir;
//     Quaternion startRotation;
//     float angle;

//     [Header("Other")]
//     Collider c;
//     public DataHandler dataHandler;
//     public float spawnTime = 2f; // Time delay between spawns
//     public int simNo;

//     void Start()
//     {
//         c = GetComponent<Collider>();
//         c.isTrigger = true;
//         direction();

//         // Start coroutine for spawning vehicles
//         StartCoroutine(SpawnVehicles());
        
//     }

//     void direction()
//     {
//         if (WayDir == null)
//     {
//         Debug.LogWarning("Waypoint direction is null! Make sure WayDir is assigned.");
//         return;
//     }
//         Waypoint w = WayDir;   
//         float z = w.transform.position.z - transform.position.z;
//         float x = w.transform.position.x - transform.position.x;
//         angle = Mathf.Atan2(x,z) * Mathf.Rad2Deg;
//         startRotation = Quaternion.Euler(0, angle, 0);
//         this.transform.rotation = startRotation;

//     }

//     IEnumerator SpawnVehicles()
//     {
//         while (true) // Infinite loop to keep spawning
//         {
//             if (CanSpawn())
//             {
//                 GameObject vClone = Instantiate(vehicleObject, transform.position, this.transform.rotation);
//                 vClone.name = "Vehicle " + id;
//                 vClone.transform.rotation = startRotation;

//                 // Assign vehicle properties
//                 Description d = vClone.GetComponent<Description>();
//                 CommunicationAgent cm = vClone.GetComponent<CommunicationAgent>();
//                 Navigation n = vClone.GetComponent<Navigation>();

//                 d.id = id;
//                 n.ID = id;
//                 n.vehicleType = type;
//                 n.DestinationSegment = endSegment;
//                 n.dest = dest;
//                 n.DestinationSegment = destSegment;
//                 n.segcycle = simNo;
//                 cm.id = id;
//                 n.CurrentSegment = startSegment;
//                 n.dataHandler = dataHandler;
//                 vClone.SetActive(true);
//             }
//             Destroy(gameObject);

//             yield return new WaitForSeconds(spawnTime); // Wait before next spawn
//         }
//     }

//     bool CanSpawn()
//     {
//         Collider[] colliders = Physics.OverlapSphere(transform.position, 1f); // Check for nearby objects
//         foreach (Collider col in colliders)
//         {
//             if (col.gameObject.tag == "AutonomousVehicle")
//             {
//                 return false; // Don't spawn if another vehicle is in range
//             }
//         }
//         return true;
//     }
// }











// using System.Collections;
// using System.Collections.Generic;
// using TrafficSimulation;
// using UnityEngine;
// using UnityEngine.U2D;
// using static Navigation;

// public class VehicleSpawnerObject : MonoBehaviour
// {

//     [Header("Vehicle Object")]
//     public GameObject vehicleObject;


//     [Header("SpawnInfo")]
//     public Timer t;
//     public int id;
//     public VehicleType type;

//     public Segment startSegment;
//     public Segment endSegment;

//     public Waypoint WayDir;
//     Quaternion startRotation;
//     float angle;

//     [Header("Other")]
//     Collider c;
//     bool canSpawn = false;
//     public float spawnTime;
//     // Start is called before the first frame update
//     void Start()
//     {
//         c = GetComponent<Collider>();
//         c.isTrigger = true;
//         direction();
//         // SetWaypointVehicleIsOn();

//     }

//     // void direction()
//     // {
//     //     float minDist = float.MaxValue;
//     //     for (int j = 0; j < startSegment.waypoints.Count; j++)
//     //     {
//     //         float d = Vector3.Distance(transform.position, startSegment.waypoints[j].transform.position);
//     //         Vector3 lSpace = transform.InverseTransformPoint(startSegment.waypoints[j].transform.position);
//     //         if (d < minDist && lSpace.z > 0)//
//     //         {

//     //             Debug.Log("Target Waypoint: " + j);
//     //             minDist = d;
//     //             Waypoint w = startSegment.waypoints[j];
//     //             float z = w.transform.position.z - transform.position.z;
//     //             float x = w.transform.position.x - transform.position.x;
//     //             angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
//     //             startRotation = Quaternion.Euler(0, angle, 0);
//     //             this.transform.rotation = startRotation;
//     //             startRotation = Quaternion.Euler(0, angle - 90, 0);

//     //             // Debug.Log("Calculated Angle: " + angle);
//     //             // Debug.Log("Start Rotation: " + startRotation.eulerAngles);
//     //         }
//     //     }
//     //     canSpawn = true;
//     // }

// //     void direction()
// // {
// //     float minDist = float.MaxValue;
// //     Vector3 forward = transform.forward; // Spawner's forward direction

// //     for (int j = 0; j < startSegment.waypoints.Count; j++)
// //     {
// //         Vector3 waypointPos = startSegment.waypoints[j].transform.position;
// //         float d = Vector3.Distance(transform.position, waypointPos);
// //         Vector3 directionToWaypoint = (waypointPos - transform.position).normalized;

// //         // Use dot product to check if waypoint is in front
// //         float dotProduct = Vector3.Dot(forward, directionToWaypoint);

// //         if (d < minDist && dotProduct > 0) // Ensures waypoint is ahead
// //         {
// //             Debug.Log("Target Waypoint: " + j);
// //             minDist = d;
// //             Waypoint w = startSegment.waypoints[j];

// //             float z = w.transform.position.z - transform.position.z;
// //             float x = w.transform.position.x - transform.position.x;
// //             angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;

// //             startRotation = Quaternion.Euler(0, angle, 0);
// //             this.transform.rotation = startRotation;
// //             startRotation = Quaternion.Euler(0, angle - 90, 0);
// //         }
// //     }
// //     canSpawn = true;
// // }


// // void direction()
// // {
// //     float minDist = float.MaxValue;
// //     Vector3 spawnerForward = transform.forward; // Default forward direction

// //     // Check segment orientation and adjust if necessary
// //     Vector3 segmentDirection = (startSegment.waypoints[startSegment.waypoints.Count - 1].transform.position 
// //                                - startSegment.waypoints[0].transform.position).normalized;
    
// //     // If the segment direction and spawner forward direction have a negative dot product, flip spawner forward
// //     if (Vector3.Dot(spawnerForward, segmentDirection) < 0)
// //     {
// //         spawnerForward = -spawnerForward; // Flip direction to match segment
// //     }

// //     for (int j = 0; j < startSegment.waypoints.Count; j++)
// //     {
// //         Vector3 waypointPos = startSegment.waypoints[j].transform.position;
// //         float d = Vector3.Distance(transform.position, waypointPos);
// //         Vector3 directionToWaypoint = (waypointPos - transform.position).normalized;

// //         // Use dot product with adjusted forward direction
// //         float dotProduct = Vector3.Dot(spawnerForward, directionToWaypoint);

// //         if (d < minDist && dotProduct > 0) // Ensures waypoint is ahead in the correct direction
// //         {
// //             Debug.Log("Target Waypoint: " + j);
// //             minDist = d;
// //             Waypoint w = startSegment.waypoints[j];

// //             float z = w.transform.position.z - transform.position.z;
// //             float x = w.transform.position.x - transform.position.x;
// //             angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;

// //             startRotation = Quaternion.Euler(0, angle, 0);
// //             this.transform.rotation = startRotation;
// //             startRotation = Quaternion.Euler(0, angle - 90, 0);
// //         }
// //     }
// //     canSpawn = true;
// // }

// // void direction()
// // {
// //     float minDist = float.MaxValue;
// //     Vector3 spawnerForward = transform.forward; // Default forward direction

// //     // Determine the local segment direction near the spawner
// //     Vector3 segmentDirection = Vector3.zero;
    
// //     for (int i = 0; i < startSegment.waypoints.Count - 1; i++)
// //     {
// //         Vector3 tempDirection = (startSegment.waypoints[i + 1].transform.position 
// //                                - startSegment.waypoints[i].transform.position).normalized;

// //         if (Vector3.Dot(spawnerForward, tempDirection) > 0) // Ensure it's moving forward
// //         {
// //             segmentDirection = tempDirection;
// //             break;
// //         }
// //     }

// //     if (segmentDirection == Vector3.zero) 
// //     {
// //         Debug.LogWarning("Could not determine a valid segment direction.");
// //         return;
// //     }

// //     // Flip direction if necessary
// //     if (Vector3.Dot(spawnerForward, segmentDirection) < 0)
// //     {
// //         spawnerForward = -spawnerForward;
// //     }

// //     for (int j = 0; j < startSegment.waypoints.Count; j++)
// //     {
// //         Vector3 waypointPos = startSegment.waypoints[j].transform.position;
// //         float d = Vector3.Distance(transform.position, waypointPos);
// //         Vector3 directionToWaypoint = (waypointPos - transform.position).normalized;

// //         // Use dot product with corrected forward direction
// //         float dotProduct = Vector3.Dot(spawnerForward, directionToWaypoint);

// //         if (d < minDist && dotProduct > 0) // Ensure waypoint is ahead
// //         {
// //             Debug.Log("Target Waypoint: " + j);
// //             minDist = d;
// //             Waypoint w = startSegment.waypoints[j];

// //             float z = w.transform.position.z - transform.position.z;
// //             float x = w.transform.position.x - transform.position.x;
// //             angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;

// //             startRotation = Quaternion.Euler(0, angle, 0);
// //             this.transform.rotation = startRotation;
// //             // startRotation = Quaternion.Euler(0, angle - 90, 0);
// //         }
// //     }
// //     canSpawn = true;
// // }


// void direction(){
//     //use waydir to determine the direction of the vehicle
//     // might be using the origin waypoint so change to next waypoint

    
//                 Waypoint w = WayDir;
//                 float z = w.transform.position.z - transform.position.z;
//                 float x = w.transform.position.x - transform.position.x;
//                 angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
//                 startRotation = Quaternion.Euler(0, angle, 0);
//                 this.transform.rotation = startRotation;
//                 // startRotation = Quaternion.Euler(0, angle - 90, 0);
//                 // Debug.Log("Cube name"+gameObject.name);
//                 // Debug.Log("Waypoint name"+w.name);
//                 canSpawn = true;

// }


//     void Update()
//     {
//         if (canSpawn)//TODO - Check if timer reached
//         {
//             GameObject vClone = Instantiate(vehicleObject, transform.position, startRotation);//TODO: Add a timer to spawn vehicles
//             canSpawn = false;
//             vClone.SetActive(true);
//             // Debug.Log("Instantiated Rotation: " + vClone.transform.rotation.eulerAngles);
//             Description d = vClone.GetComponent<Description>();
//             Navigation n = vClone.GetComponent<Navigation>();
//             CommunicationAgent cm = vClone.GetComponent<CommunicationAgent>();
//             d.id = id;
//             n.vehicleType = type;
//             // n.CurrentSegment = startSegment;
//             n.DestinationSegment = endSegment;
//             cm.id = id;
//             //Destroy Self
//             // Destroy(gameObject);
//         }
//     }
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject.tag == "AutonomousVehicle")
//         {
//             canSpawn = false;
//         }
//         else
//         {
//             canSpawn = true;
//         }
//     }
//     void OnTriggerExit(Collider other)
//     {
//         if (other.gameObject.tag == "AutonomousVehicle")
//         {
//             canSpawn = true;
//         }
//         else
//         {
//             canSpawn = false;
//         }

//     }
//     void OnTriggerStay(Collider other)
//     {
//         if (other.gameObject.tag == "AutonomousVehicle")
//         {
//             canSpawn = false;
//         }
//         else
//         {
//             canSpawn = true;
//         }
//     }

//     void SetWaypointVehicleIsOn()
//     {
//         int targetWaypoint = 0;

//         //Find nearest waypoint to start within the segment
//         float minDist = float.MaxValue;
//         for (int j = 0; j < startSegment.waypoints.Count; j++)
//         {
//             Debug.Log("Waypoint: " + j);
//             float d = Vector3.Distance(this.transform.position, startSegment.waypoints[j].transform.position);

//             //Only take in front points
//             Vector3 lSpace = this.transform.InverseTransformPoint(startSegment.waypoints[j].transform.position);
//             if (d < minDist && lSpace.z > 0)
//             {
//                 minDist = d;
//                 targetWaypoint = j;
//             }
//         }
//         Debug.ClearDeveloperConsole();
//         Debug.Log("Target Waypoint: " + targetWaypoint);

//     }
// }
