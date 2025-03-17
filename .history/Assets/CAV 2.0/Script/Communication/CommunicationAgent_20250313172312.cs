using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class CommunicationAgent : MonoBehaviour
{
    //SECTION: Variables
    //communication system
    public CommunicationSystem communicationSystem;
    public int id;

    public void Awake(){
        communicationSystem = FindObjectOfType<CommunicationSystem>();
        if(communicationSystem == null){
            Debug.LogError("CommunicationSystem not found in scene.");
        }
    }

    //SECTION: Main
    public void Update(){
        if(id == 0){
            SendMessageToVehicle("Hello", id, 1);
        }
    }

    //!SECTION: Methods
    //Receive message
    public void ReceiveInformation(string message, int senderId, int receiverId)
    {
        Debug.Log("Message received: " + message + " from: " + senderId + " to: " + receiverId);
    }
    //Send message to vehicle
    public void SendMessageToVehicle(string message, int senderId, int receiverId)
    {
        communicationSystem.SendInformationToVehicle(senderId, receiverId, message);
    }
    //send message to road
    public void SendMessageToRoad(string message, int senderId, int roadId)
    {
        communicationSystem.SendInformationToRoad(senderId, roadId, message);
    }
    //send messsage to update road
    public void SendMessageToUpdateRoad(string message, int senderId, int roadId)
    {
        communicationSystem.SendDataUpdateToRoad(senderId, roadId, message);
    }

    //NAvigation
    public Segment getCurrentSegment(Vector3 position)
    {
        Segment segment = communicationSystem.GetCurrentSegment(position);
        if(segment == null){
            segment = communicationSystem.GetNearbySegment(position);//NOTE - if not on segment get nearest segment
        }
        return segment;
    }

    //entering segment
    public void EnterSegment(Segment segment)
    {
        Debug.Log("Entering segment: " + segment.id);
        // communicationSystem.SendInformationToRoad(id, segment.id, "Entering segment");
        // communicationSystem.roadSegments.Find(x => x.roadId == segment.id).vehicles.Add(id);
        // communicationSystem.roadSegments.Find(x => x.roadId == segment.id).trafficDensity++;
        // FIXME - the above code is not working something wrong with the id matching of the segment and the ROAD segment objects
    }

    //exiting segment
    public void ExitSegment(Segment segment)
    {
        Debug.Log("Exiting segment: " + segment.id);
        communicationSystem.SendInformationToRoad(id, segment.id, "Exiting segment");
        communicationSystem.roadSegments.Find(x => x.roadId == segment.id).vehicles.Remove(id);
        communicationSystem.roadSegments.Find(x => x.roadId == segment.id).vehicleData.Remove(communicationSystem.roadSegments.Find(x => x.roadId == segment.id).vehicleData.Find(x => x.vehicleId == id));
        communicationSystem.roadSegments.Find(x => x.roadId == segment.id).trafficDensity--;
    }
}


