using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using namespace trafficSystem;
public class StaticAStar : MonoBehaviour
{
//     public TrafficSystem trafficSystem;
//     public List<Segment> Map;
//     public List<Segment> path;
//     public Segment CurrentSegment;
//     public Segment DestinationSegment;
//     public List<Segment> SegmentPathList;
//     public int CurrentSegmentCost;
//     public int ID;
//     public Vehicle_AI Vehicle_AI;
//     public List<int> path;
//     public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
//     public Segment CurrentSegment;
//     public int CurrentSegmentCost;
//     public Segment DestinationSegment;
//     // public List<Segment> Map;
//     // public SimConfig.VehicleType vehicleType;
//     // public List<Segment> path;

//     // public Vector3 currentPos;
//     // public Segment currentSegment;
//     // public Segment nextSegment;

//     // public Vehicle_AI thisAI;

//     // public CarAI carAI;

//     private void Start()
//     {
//         //At start
//         UpdateCurrentSegment();

//         // // Initialize components and variables
//         // thisAI = GetComponent<Vehicle_AI>();
//         // comagent = GetComponent<CommunicationAgent>();

//         // // Get current position and segment
//         // currentPos = transform.position;
//         // currentSegment = comagent.getCurrentSegment(currentPos);

//         // if (currentSegment == null)
//         // {
//         //     Debug.LogError("Failed to determine the current segment.");
//         //     return;
//         // }

//         // Debug.Log($"Current segment: {currentSegment.id}");
//         // comagent.EnterSegment(currentSegment);

//         // // Generate initial path
//         // path = RandomPath(currentSegment);

//         // if (path.Count > 1)
//         // {
//         //     nextSegment = path[1];
//         // }
    }

    private void Update()
    {
        // if (currentSegment == null)
        // {
        //     Debug.LogError("Failed to determine the current segment.");
        //     return;
        // }

        // if (currentSegment == DestinationSegment)
        // {
        //     Debug.Log("Reached destination.");
        //     return;
        // }

        // if (currentSegment == nextSegment)
        // {
        //     path.RemoveAt(0);
        //     if (path.Count > 1)
        //     {
        //         nextSegment = path[1];
        //     }
        //     else
        //     {
        //         Debug.Log("Reached destination.");
        //         return

// public class StaticAStar : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }


// A star


// public class Segment
// {
//     public int ID;
//     public float Cost;
//     public List<int> NextSegments;

//     public Segment(int id, float cost, List<int> nextSegments = null)
//     {
//         ID = id;
//         Cost = cost;
//         NextSegments = nextSegments ?? new List<int>();
//     }
// }

public class AStarPathfinder
{
    private Dictionary<int, Segment> segments;

    public AStarPathfinder(Dictionary<int, Segment> segments)
    {
        this.segments = segments;
    }

    private float Heuristic(int current, int goal)
    {
        return 0f; // Placeholder (you can replace it with a better heuristic)
    }

    public List<int> FindPath(int startID, int goalID)
    {
        if (!segments.ContainsKey(startID) || !segments.ContainsKey(goalID))
            return null; // Invalid start or goal

        PriorityQueue<int, float> openSet = new PriorityQueue<int, float>();
        openSet.Enqueue(startID, 0f);

        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        Dictionary<int, float> gScore = segments.Keys.ToDictionary(id => id, id => float.MaxValue);
        gScore[startID] = 0f;

        while (openSet.Count > 0)
        {
            int currentID = openSet.Dequeue();

            if (currentID == goalID)
                return ReconstructPath(cameFrom, startID, goalID);

            foreach (int nextID in segments[currentID].NextSegments)
            {
                float tentativeGScore = gScore[currentID] + segments[nextID].Cost;

                if (tentativeGScore < gScore[nextID])
                {
                    cameFrom[nextID] = currentID;
                    gScore[nextID] = tentativeGScore;
                    float fScore = tentativeGScore + Heuristic(nextID, goalID);
                    openSet.Enqueue(nextID, fScore);
                }
            }
        }

        return null; // No path found
    }

    private List<int> ReconstructPath(Dictionary<int, int> cameFrom, int startID, int goalID)
    {
        List<int> path = new List<int> { goalID };
        while (path.Last() != startID)
        {
            path.Add(cameFrom[path.Last()]);
        }
        path.Reverse();
        return path;
    }
}

// Unity-compatible Priority Queue (Min Heap)
public class PriorityQueue<TElement, TPriority> where TPriority : System.IComparable<TPriority>
{
    private List<(TElement element, TPriority priority)> elements = new List<(TElement, TPriority)>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add((element, priority));
        elements.Sort((x, y) => x.priority.CompareTo(y.priority)); // Sort by priority
    }

    public TElement Dequeue()
    {
        var item = elements[0].element;
        elements.RemoveAt(0);
        return item;
    }
}