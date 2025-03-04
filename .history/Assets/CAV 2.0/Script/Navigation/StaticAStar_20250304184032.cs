using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TrafficSimulation;

public class StaticAStar 
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

