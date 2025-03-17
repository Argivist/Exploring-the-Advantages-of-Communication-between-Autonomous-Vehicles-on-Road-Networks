using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class CommunicationSystem : MonoBehaviour
{
    public TrafficSystem trafficSystem;
    public int roadCount;
    public List<RoadSegment> roadSegments;

    // Road Segment class
    public class RoadSegment
    {
        public int roadId;
        public Vector3 start;
        public Vector3 end;
        public float length;
        public int trafficDensity;
        public List<int> vehicles;

        public float dCost;
        public int avgTime;

        public RoadSegment(int roadId, Vector3 start, Vector3 end, float length)
        {
            this.roadId = roadId;
            this.start = start;
            this.end = end;
            this.length = length;
            this.trafficDensity = 0;
            this.vehicles = new List<int>();
        }

        public void AddVehicle(int vehicleId)
        {
            if (!vehicles.Contains(vehicleId))
            {
                vehicles.Add(vehicleId);
                trafficDensity++;
            }
        }

        public void RemoveVehicle(int vehicleId)
        {
            if (vehicles.Remove(vehicleId))
            {
                trafficDensity--;
            }
        }

        public bool FindVehicle(int vehicleId)
        {
            return vehicles.Contains(vehicleId);
        }

        public void ReceiveInformation(string message, int senderId)
        {
            Debug.Log($"Message received on road {roadId}: '{message}' from sender {senderId}");
        }

        public float updateCost(){
            return dCost;
        }
    }


    // Start
    private void Start()
    {
        if (trafficSystem == null)
        {
            Debug.LogError("TrafficSystem not assigned!");
            return;
        }

        roadSegments = new List<RoadSegment>();

        foreach (Segment ts in trafficSystem.segments)
        {
            float segmentLength = Vector3.Distance(
                ts.waypoints[0].transform.position,
                ts.waypoints[ts.waypoints.Count - 1].transform.position
            );

            roadSegments.Add(new RoadSegment(
                ts.id,
                ts.waypoints[0].transform.position,
                ts.waypoints[ts.waypoints.Count - 1].transform.position,
                segmentLength
            ));
        }

        roadCount = roadSegments.Count;
    }

    // Send information to a vehicle
    public void SendInformationToVehicle(int sender, int receiver, string message)
    {
        GameObject vehicle = GameObject.Find($"Vehicle {receiver}");
        if (vehicle != null && vehicle.TryGetComponent(out CommunicationAgent agent))
        {
            agent.ReceiveInformation(message, sender, receiver);
        }
        else
        {
            Debug.LogError($"Vehicle with ID {receiver} not found or missing CommunicationAgent!");
        }
    }

    // Send information to a road
    public void SendInformationToRoad(int sender, int roadId, string message)
    {
        RoadSegment road = roadSegments.Find(x => x.roadId == roadId);
        if (road != null)
        {
            road.ReceiveInformation(message, sender);
        }
        else
        {
            Debug.LogError($"Road with ID {roadId} not found!");
        }
    }

    public RoadSegment GetRoadObject(int i){
        foreach (RoadSegment rs in roadSegments)
        {
            if (rs.roadId == i)
            {
                return rs;
            }
            return null;
        }
    }

    // Get the current segment of a vehicle
    public Segment GetCurrentSegment(Vector3 pos)
    {
        // foreach (Segment segment in trafficSystem.segments)
        // {
        //     if (segment.IsOnSegment(pos))
        //     {
        //         return segment;
        //     }
        // }


        Debug.LogWarning("Current segment not found for position: " + pos);
        return trafficSystem.segments[0];
    }

    // Get the nearest segment to a position
    public Segment GetNearbySegment(Vector3 pos)
    {
        Segment nearestSegment = null;
        float minDistance = Mathf.Infinity;

        foreach (Segment segment in trafficSystem.segments)
        {
            float distance = segment.DistanceFromSegment(pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSegment = segment;
            }
        }

        return nearestSegment;
    }
}
