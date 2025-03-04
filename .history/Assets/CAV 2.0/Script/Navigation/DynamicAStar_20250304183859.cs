using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TrafficSimulation;

public class DynamicAStar 
{
    private List<Segment> segments;

    public void AStarPathfinder(List<Segment> segments)
    {
        this.segments = segments;
    }

    private float Heuristic(int current, int goal)
    {
        return 0f; // Placeholder for better heuristic
    }

    public List<int> FindPath(int startID, int goalID)
    {
        Segment startSegment = segments.FirstOrDefault(s => s.id == startID);
        Segment goalSegment = segments.FirstOrDefault(s => s.id == goalID);

        if (startSegment == null || goalSegment == null)
            return null; // Invalid start or goal

        PriorityQueue<int, float> openSet = new PriorityQueue<int, float>();
        openSet.Enqueue(startID, 0f);

        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        Dictionary<int, float> gScore = segments.ToDictionary(seg => seg.id, seg => float.MaxValue);
        gScore[startID] = 0f;

        while (openSet.Count > 0)
        {
            int currentID = openSet.Dequeue();
            Segment currentSegment = segments.FirstOrDefault(s => s.id == currentID);

            if (currentSegment == null)
                continue; // Skip if segment is missing

            if (currentID == goalID)
                return ReconstructPath(cameFrom, startID, goalID);

            foreach (Segment nextSegment in currentSegment.nextSegments) // Iterate through actual Segment objects
            {
                int nextID = nextSegment.id;  
                float tentativeGScore = gScore[currentID] + nextSegment.cost; 

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