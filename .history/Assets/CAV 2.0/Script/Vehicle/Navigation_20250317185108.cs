using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using UnityEditor.SceneManagement;
using NUnit.Framework.Constraints;
using System.Xml.Serialization;

public class Navigation : MonoBehaviour
{

    [Header("Vehicle Configure")]
    public Vehicle_AI Vehicle_AI;
    public StopWatch sw;
    public StopWatch segmentsw;
    public GameObject Vehicle;
    public Vector3 dest;
    public float speed;
    public CommunicationAgent communicationAgent;


    [Header("Traffic System")]
    public TrafficSystem trafficSystem;


    [Header("Communication")]
    public int ID;

    [Header("Navigation")]
    public float DistanceToDestination;
    public float destinationThreshold = 1;
    public List<int> path;
    // public List<Segment> SegmentPathList;//Temporary remove when navigator proved to work
    public Segment CurrentSegment;
    public float CurrentSegmentCost;
    public Segment DestinationSegment;


    StaticAStar staticAStar;
    DynamicAStar dynamicAStar;

    // Vehicle share info
    Vector3 positionOfPrevFrame;
    Vector3 positionOfCurrentFrame;

    int prevTime;
    int currentTime;
    // vehicle type
    public enum VehicleType
    {
        CAV,
        NonCAV
    }
    public VehicleType vehicleType;
    
























}