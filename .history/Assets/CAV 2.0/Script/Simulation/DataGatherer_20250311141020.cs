using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using static SimConfig;

public class DataGatherer : MonoBehaviour
{
    // struct VehicleObject{
    //     int id;
    //     Vehicle vehicle;
    //     int time;

    //     public VehicleObject(Vehicle v){
    //         this.id=v.vehicleId;
    //         this.vehicle=v;
    //     }
    //     public void recordTime(){

    //     }
    // }

    [System.Serializable]
    public class VehicleLog
    {
        public string vehicleName;
        public int simulation;
        public int logTime;
        public Vector3 logPosition;
        public Segment logSegment;
        public LogAction logAction;
        public float logSpeed;
        public float logAcceleration;

        public VehicleLog(string vehicleName, int simulation, int logTime, Vector3 logPosition, Segment logSegment, LogAction logAction, float logSpeed, float logAcceleration)
        {
            this.vehicleName = vehicleName;
            this.simulation = simulation;
            this.logTime = logTime;
            this.logPosition = logPosition;
            this.logSegment = logSegment;
            this.logAction = logAction;
            this.logSpeed = logSpeed;
            this.logAcceleration = logAcceleration;
        }
    }
    // List<VehicleObject> Vlist;
    public List<VehicleLog> vehicleLogList = new List<VehicleLog>();


    // public void AddToVlist(Vehicle v){
    //     Vlist.Add(new VehicleObject(v));
    // }
}
