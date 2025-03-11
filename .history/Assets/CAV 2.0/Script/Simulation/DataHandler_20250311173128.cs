using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;

    class VehicleData{
        int id;
        List<int> time_spent;
        List<int> simulation;

        Vector3 StartPosition;
        Vector3 EndPosition;
        VehicleType type;
        VehicleData(int id,Vector3 StartPosition,Vector3 EndPosition,VehicleType type){
            this.id=id;
            this.StartPosition=StartPosition;
            this.EndPosition=EndPosition;
            this.type=type;
        }

        public void recordTime(int time,int simulation){
            time_spent.Add(time);
            this.simulation.Add(simulation);
        }

    }

public class DataHandler : MonoBehaviour
{
    public List<VehicleData> vehicleList;
    [Header("Data Handler")]
    public bool isDataProcessed=true;




}
