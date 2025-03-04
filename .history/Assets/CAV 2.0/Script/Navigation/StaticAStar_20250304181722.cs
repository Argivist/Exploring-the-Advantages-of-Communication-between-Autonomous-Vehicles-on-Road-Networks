using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using namespace trafficSystem;
public class StaticAStar : MonoBehaviour
{
    
}

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