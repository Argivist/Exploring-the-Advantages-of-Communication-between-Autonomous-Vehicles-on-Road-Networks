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
        // Data Handler
        DataHandler cloneDH=Instantiate(dh);
        cloneDH.gameObject.SetActive(true);

        // Simulation Configurations
        SimulationConfigurer cloneConfig=Instantiate(sc);
        cloneConfig.VehicleDensity=SimGroups[i];
        // set any aditional configurations here    
        cloneConfig.gameObject.SetActive(true);

        // Simulation Master
        SimulationMaster_ cloneSM=Instantiate(sm);
        cloneSM.gameObject.SetActive(true);
    }

}
