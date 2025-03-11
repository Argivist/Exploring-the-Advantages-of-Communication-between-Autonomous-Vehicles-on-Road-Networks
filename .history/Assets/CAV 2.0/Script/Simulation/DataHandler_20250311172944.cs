using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;

public class DataHandler : MonoBehaviour
{
    class Vehicle{
        int id;
        List<int> time_spent;
        List<int> simulation;

        Vector3 StartPosition;
        Vector3 EndPosition;
        VehicleType type;
        Vehicle(int id,Vector3 StartPosition,Vector3 EndPosition,VehicleType type){
            this.id=id;
            this.StartPosition=StartPosition;
            this.EndPosition=EndPosition;
            this.type=type;
        }

        re

    }

    [Header("Data Handler")]
    public bool isDataProcessed=true;




}
