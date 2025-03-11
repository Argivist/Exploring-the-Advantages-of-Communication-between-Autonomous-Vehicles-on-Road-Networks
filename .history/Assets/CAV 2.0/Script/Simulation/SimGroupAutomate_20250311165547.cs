using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGroupAutomate : MonoBehaviour
{
    public List<int> SimGroups;

    public SimulationConfigurer sc;
    public SimulationMaster_ sm;

    public DataHandler dh;

    void Start(){

    }


    void StartSim(int i){
        SimulationConfigurer cloneConfig=Instantiate(sc);
        cloneConfig.VehicleDensity=SimGroups[i];
        // set any aditional configurations here    
        cloneConfig.gameObject.SetActive(true);
    }

}
