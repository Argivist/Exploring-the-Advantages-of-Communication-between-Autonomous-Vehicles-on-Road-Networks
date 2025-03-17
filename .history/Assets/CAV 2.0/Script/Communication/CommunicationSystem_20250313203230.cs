using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class CommunicationSystem : MonoBehaviour
{
    public TrafficSystem trafficSystem;
    public List<RoadSegment> roadSegments;
    public int roadCount;
    public float totallength;

    private void Start()
    {
        if (trafficSystem == null)
        {
            Debug.LogError("TrafficSystem not assigned!");
            return;
        }

        InitializeRoadSegments();
    }

    private void InitializeRoadSegments()
    {
        totallength = 0;
        roadSegments = new List<RoadSegment>();
        foreach (Segment segment in trafficSystem.segments)
        {
            float segmentLength = Vector3.Distance(
                segment.waypoints[0].transform.position,
                segment.waypoints[segment.waypoints.Count - 1].transform.position
            );
            roadSegments.Add(new RoadSegment(segment.id, segment.waypoints[0].transform.position, segment.waypoints[segment.waypoints.Count - 1].transform.position, segmentLength));
            totallength += segmentLength;
        }
        foreach (RoadSegment rs in roadSegments)
        {
            rs.setTotalLength(totallength);
        }
        roadCount = roadSegments.Count;
    }

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
    public void SendDataUpdateToRoad(int vehicleId, int roadId, float speed, int time)
    {
        RoadSegment road = roadSegments.Find(x => x.roadId == roadId);
        if (road != null)
        {
            road.UpdateData(vehicleId, speed, time);
        }
        else
        {
            Debug.LogError($"Road with ID {roadId} not found!");
        }
    }

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

    public RoadSegment GetRoadObject(int roadId)
    {
        return roadSegments.Find(rs => rs.roadId == roadId);
    }


public Segment GetCurrentSegment(Vector3 position)
{
    if (trafficSystem.segments.Count == 0)
    {
        Debug.LogWarning("No segments available in traffic system.");
        return null;
    }

    return trafficSystem.segments[0]; // Default behavior
}


    // public Segment GetCurrentSegment(Vector3 position)
    // {
    //     Debug.LogWarning("Current segment detection not implemented.");
    //     return trafficSystem.segments[0];
    // }

    public Segment GetNearbySegment(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        Segment nearestSegment = null;

        foreach (Segment segment in trafficSystem.segments)
        {
            float distance = segment.DistanceFromSegment(position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSegment = segment;
            }
        }

        return nearestSegment;
    }

    public class RoadSegment
    {
        public int roadId;
        public Vector3 start, end;
        public float length, totalLength;
        public int trafficDensity;
        public List<int> vehicles;
        public List<VehicleData> vehicleData;
        public float avgSpeed, avgTime;
        public float dynamicCost;
        private const float maxSpeed = 0.04f;

        public RoadSegment(int roadId, Vector3 start, Vector3 end, float length)
        {
            this.roadId = roadId;
            this.start = start;
            this.end = end;
            this.length = length;
            trafficDensity = 0;
            vehicles = new List<int>();
            vehicleData = new List<VehicleData>();
        }
        public void setTotalLength(float l)
        {
            totalLength += l;
        }

        public void AddVehicle(int vehicleId)
        {
            if (!vehicles.Contains(vehicleId))
            {
                vehicles.Add(vehicleId);
                vehicleData.Add(new VehicleData(vehicleId, 0, 0));
                trafficDensity++;
            }
        }

        public void RemoveVehicle(int vehicleId)
        {
            if (vehicles.Remove(vehicleId))
            {
                vehicleData.RemoveAll(v => v.vehicleId == vehicleId);
                trafficDensity--;
            }
        }

    public void UpdateData(int vehicleId, float speed, int time)
{
    VehicleData data = vehicleData.Find(v => v.vehicleId == vehicleId);
    if (data == null)
    {
        AddVehicle(vehicleId);
        data = vehicleData.Find(v => v.vehicleId == vehicleId);
    }

    data.speed = speed;
    data.time = time;
    UpdateAverages();

    Debug.Log($"Data updated for vehicle {vehicleId} on road {roadId}");
    Debug.Log($"Speed: {speed}, Time: {time}, AvgSpeed: {avgSpeed}, AvgTime: {avgTime}");

    if (totalLength > 0) 
    {
        float expectedTime = length / maxSpeed;
        if(avgTime<expectedTime){
            avgTime=expectedTime;
        }
        // dynamicCost = (length / totalLength) * (1-(1-(avgTime / (length / maxSpeed))));
        dynamicCost = (length / totalLength) * (avgTime / (length / maxSpeed));
    }
}

        // public void UpdateData(int vehicleId, float speed, int time)
        // {
        //     VehicleData data = vehicleData.Find(v => v.vehicleId == vehicleId);
        //     if (data != null)
        //     {
        //         data.speed = speed;
        //         data.time = time;
        //         UpdateAverages();
        //         Debug.Log($"Data updated for vehicle {vehicleId} on road {roadId}");
        //         // data values used in dynamic cost calculation
        //         Debug.Log($"Speed: {speed}, Time: {time}, AvgSpeed: {avgSpeed}, AvgTime: {avgTime}");
        //         dynamicCost = (length / totalLength) * (avgTime / (length / maxSpeed)) ;
        //     }else{
        //         Debug.LogWarning($"Vehicle {vehicleId} not found on road {roadId}");
        //         //add vehicle to road
        //         AddVehicle(vehicleId);
        //         //update data
        //         UpdateData(vehicleId, speed, time);
        //     }
        // }

        public float UpdateCost()
        {
            return dynamicCost;
        }
        public int getTrafficDensity()
        {
            return trafficDensity;
        }

        private void UpdateAverages()
        {
            if (vehicleData.Count == 0) return;

            avgTime = 0;
            avgSpeed = 0;
            foreach (VehicleData data in vehicleData)
            {
                avgTime += data.time;
                avgSpeed += data.speed;
            }
            avgTime /= vehicleData.Count;
            avgSpeed /= vehicleData.Count;
        }

public void ReceiveInformation(string message, int senderId)
{
    Debug.Log($"Message received on road {roadId}: '{message}' from sender {senderId}");

    if (message == "enter")
    {
        AddVehicle(senderId);
    }
    else if (message == "exit")
    {
        if (vehicles.Contains(senderId))
        {
            RemoveVehicle(senderId);
        }
        else
        {
            Debug.LogWarning($"Attempted to remove vehicle {senderId}, but it was not on road {roadId}");
        }
    }
}


    //     public void ReceiveInformation(string message, int senderId)
    //     {
    //         Debug.Log($"Message received on road {roadId}: '{message}' from sender {senderId}");
    //         if (message == "enter")
    //         {
    //             AddVehicle(senderId);

    //             }
            
    //         else if (message == "exit")
    //         {
    //             RemoveVehicle(senderId);
    //             }
            
    //     }

    }

    public class VehicleData
    {
        public int vehicleId;
        public float speed;
        public int time;

        public VehicleData(int vehicleId, float speed, int time)
        {
            this.vehicleId = vehicleId;
            this.speed = speed;
            this.time = time;
        }
    }
}





// using System.Collections.Generic;
// using UnityEngine;
// using TrafficSimulation;

// public class CommunicationSystem : MonoBehaviour
// {

//     public class vData{
//         public int vehicleId;
//         public float speed;
//         public int time;

//         public vData(int vehicleId, float speed, int time){
//             this.vehicleId=vehicleId;
//             this.speed=speed;
//             this.time=time;
//         }
//     }
//     //TODO: caclulate rod lenght
//     public TrafficSystem trafficSystem;
//     public int roadCount;
//     public List<RoadSegment> roadSegments;

//     // Road Segment class
//     public class RoadSegment
//     {
//         public int roadId;
//         public Vector3 start;
//         public Vector3 end;
//         public float length;
//         public float totalLength;
//         public int trafficDensity;
//         public List<int> vehicles;
//         public List<vData> vehicleData;

//         public float dCost=1;
//         public int avgTime;
//         public float avgSpeed;
//         float maxSpeed=0.04f;

//         public RoadSegment(int roadId, Vector3 start, Vector3 end, float length)
//         {
//             this.roadId = roadId;
//             this.start = start;
//             this.end = end;
//             this.length = length;
//             this.trafficDensity = 0;
//             this.vehicles = new List<int>();
//             this.vehicleData = new List<vData>();

//         }



//         public void AddVehicle(int vehicleId)
//         {
//             if (!vehicles.Contains(vehicleId))
//             {
//                 vehicles.Add(vehicleId);
//                 vehicleData.Add(new vData(vehicleId, 0, 0));
//                 trafficDensity++;
//             }
//         }

//         public void RemoveVehicle(int vehicleId)
//         {
//             if (vehicles.Remove(vehicleId))
//             {
//                 vehicleData.Remove(vehicleData.Find(x => x.vehicleId == vehicleId));
//                 trafficDensity--;
//             }
//         }
//         public void updateData(int vehicleId, float speed, int time){
//             vehicleData.Find(x => x.vehicleId == vehicleId).speed=speed;
//             vehicleData.Find(x => x.vehicleId == vehicleId).time=time;
//             updateAvgTime();
//             updateAvgSpeed();
//         }
//         public void updateAvgTime(){
//             int sum=0;
//             int count=0;
//             foreach (int vehicle in vehicles)
//             {
//                 sum+=vehicleData.Find(x => x.vehicleId == vehicle).time;
//                 count++;   
//             }
//             avgTime=sum/count;
//         }
//         public void updateAvgSpeed(){
//             float sum=0;
//             int count=0;
//             foreach (int vehicle in vehicles)
//             {
//                 sum+=vehicleData.Find(x => x.vehicleId == vehicle).speed;
//                 count++;   
//             }
//             avgSpeed=sum/count;
//         }

//         public bool FindVehicle(int vehicleId)
//         {
//             return vehicles.Contains(vehicleId);
//         }

//         public void ReceiveInformation(string message, int senderId)
//         {
//             Debug.Log($"Message received on road {roadId}: '{message}' from sender {senderId}");
//             if(message=="enter"){
//                 AddVehicle(senderId);
//             }else if(message=="exit"){
//                 RemoveVehicle(senderId);
//             }
//         }

//         public float updateCost(){
//             return dCost;
//         }
//     }


//     // Start
//     private void Start()
//     {
//         if (trafficSystem == null)
//         {
//             Debug.LogError("TrafficSystem not assigned!");
//             return;
//         }

//         roadSegments = new List<RoadSegment>();

//         foreach (Segment ts in trafficSystem.segments)
//         {
//             float segmentLength = Vector3.Distance(
//                 ts.waypoints[0].transform.position,
//                 ts.waypoints[ts.waypoints.Count - 1].transform.position
//             );

//             roadSegments.Add(new RoadSegment(
//                 ts.id,
//                 ts.waypoints[0].transform.position,
//                 ts.waypoints[ts.waypoints.Count - 1].transform.position,
//                 segmentLength
//             ));
//         }

//         roadCount = roadSegments.Count;
//     }

//     // Send information to a vehicle
//     public void SendInformationToVehicle(int sender, int receiver, string message)
//     {
//         GameObject vehicle = GameObject.Find($"Vehicle {receiver}");
//         if (vehicle != null && vehicle.TryGetComponent(out CommunicationAgent agent))
//         {
//             agent.ReceiveInformation(message, sender, receiver);
//         }
//         else
//         {
//             Debug.LogError($"Vehicle with ID {receiver} not found or missing CommunicationAgent!");
//         }
//     }

//     // Send information to a road
//     public void SendInformationToRoad(int sender, int roadId, string message)
//     {
//         RoadSegment road = roadSegments.Find(x => x.roadId == roadId);
//         if (road != null)
//         {
//             road.ReceiveInformation(message, sender);
//         }
//         else
//         {
//             Debug.LogError($"Road with ID {roadId} not found!");
//         }
//     }

//     public RoadSegment GetRoadObject(int i){
//         foreach (RoadSegment rs in roadSegments)
//         {
//             if (rs.roadId == i)
//             {
//                 return rs;
//             }
//         }
//             return null;
//     }

//     // Get the current segment of a vehicle
//     public Segment GetCurrentSegment(Vector3 pos)
//     {
//         // foreach (Segment segment in trafficSystem.segments)
//         // {
//         //     if (segment.IsOnSegment(pos))
//         //     {
//         //         return segment;
//         //     }
//         // }


//         Debug.LogWarning("Current segment not found for position: " + pos);
//         return trafficSystem.segments[0];
//     }

//     // Get the nearest segment to a position
//     public Segment GetNearbySegment(Vector3 pos)
//     {
//         Segment nearestSegment = null;
//         float minDistance = Mathf.Infinity;

//         foreach (Segment segment in trafficSystem.segments)
//         {
//             float distance = segment.DistanceFromSegment(pos);
//             if (distance < minDistance)
//             {
//                 minDistance = distance;
//                 nearestSegment = segment;
//             }
//         }

//         return nearestSegment;
//     }
// }
