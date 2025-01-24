using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class Navigator : MonoBehaviour
{
    public TrafficSystem trafficSystem;

    public Segment currentSegment;
    public Segment destinationSegment;

    private List<Segment> path; // The final path
    private Dictionary<Segment, Segment> cameFrom; // To reconstruct the path
    private Dictionary<Segment, float> gScore; // Cost from start to current segment
    private Dictionary<Segment, float> fScore; // Estimated total cost (gScore + heuristic)

    void Start()
    {
        path = new List<Segment>();
        cameFrom = new Dictionary<Segment, Segment>();
        gScore = new Dictionary<Segment, float>();
        fScore = new Dictionary<Segment, float>();
    }

    void Update()
    {
        // If there's a path, follow it
        if (path.Count > 0)
        {
            FollowPath();
        }
    }

    public int GetNextSegment()
    {
        if (path.Count > 0)
        {
            int segmentId = path[0].id;
            path.RemoveAt(0);
            return segmentId;
        }
        Debug.LogWarning("No more segments in the path.");
        return -1;
    }

    public List<Segment> GetPath(Segment startSegment, Segment endSegment)
    {
        if (startSegment == null || endSegment == null)
        {
            Debug.LogError("Start or end segment is null. Cannot calculate path.");
            return new List<Segment>();
        }

        currentSegment = startSegment;
        destinationSegment = endSegment;

        path = AStarPathfinder(currentSegment, destinationSegment);

        if (path.Count == 0)
        {
            Debug.LogWarning("No path found between the given segments.");
        }

        return path;
    }

    public List<Segment> AStarPathfinder(Segment start, Segment end)
    {
        List<Segment> openList = new List<Segment>();
        HashSet<Segment> closedList = new HashSet<Segment>();

        gScore.Clear();
        fScore.Clear();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        openList.Add(start);

        while (openList.Count > 0)
        {
            Segment current = GetLowestFScoreSegment(openList);

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Segment neighbor in current.nextSegments)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + current.cost;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);
            }
        }

        Debug.LogWarning("A* could not find a path.");
        return new List<Segment>();
    }

    private float Heuristic(Segment segment, Segment end)
    {
        return Vector3.Distance(segment.transform.position, end.transform.position);
    }

    private Segment GetLowestFScoreSegment(List<Segment> openList)
    {
        Segment lowest = openList[0];
        foreach (Segment segment in openList)
        {
            if (fScore.ContainsKey(segment) && fScore[segment] < fScore[lowest])
            {
                lowest = segment;
            }
        }
        return lowest;
    }

    private List<Segment> ReconstructPath(Dictionary<Segment, Segment> cameFrom, Segment current)
    {
        List<Segment> path = new List<Segment>();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        path.Insert(0, current); // Add the start segment to the path
        return path;
    }

    private void FollowPath()
    {
        if (path.Count == 0)
        {
            Debug.Log("Path is empty. No movement required.");
            return;
        }

        Segment nextSegment = path[0];

        if (nextSegment.IsOnSegment(transform.position))
        {
            path.RemoveAt(0);
            currentSegment = nextSegment;

            if (path.Count > 0)
            {
                Debug.Log($"Moving to next segment: {path[0].id}");
            }
            else
            {
                Debug.Log("Reached destination segment.");
            }
        }
    }
}
