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


    void StartSim(){
        SimulationConfigurer cloneConfig=Instantiate(sc);
        cloneConfig.
    }

}
