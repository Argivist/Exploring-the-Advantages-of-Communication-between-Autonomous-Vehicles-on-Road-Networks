using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class InfoBox : MonoBehaviour
{
    //!SECTION: Variables
    public string vehicleName;
    public SimConfig.VehicleType vehicleType;
    public int vehicleId;

    public void setInfo(string vehicleName, SimConfig.VehicleType vehicleType, int vehicleId){
        this.vehicleName = vehicleName;
        this.vehicleType = vehicleType;
        this.vehicleId = vehicleId;
        //set the name of the current vehicle
        gameObject.name = vehicleName;
    }
}
