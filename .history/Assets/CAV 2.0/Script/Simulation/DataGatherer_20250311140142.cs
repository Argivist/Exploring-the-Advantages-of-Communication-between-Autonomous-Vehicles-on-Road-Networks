using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGatherer : MonoBehaviour
{
    struct VehicleObject{
        SimConfig.Vehicle vehicle;
        int time;
    }

    List<VehicleObject> Vlist;
    void Awake(){
        
    }
}
