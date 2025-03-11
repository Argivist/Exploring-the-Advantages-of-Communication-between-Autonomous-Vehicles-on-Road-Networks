using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

public class SimGroupAutomate : MonoBehaviour
{

    [Header("Simulation Groups")]
    public string Location;
    public List<int> SimGroups;

    public SimulationConfigurer sc;
    public SimulationMaster_ sm;

    public DataHandler dh;


    GameObject SimObject;
    public int group_index=0;

    void Start(){
        
        StartSim(group_index);
    }


    void StartSim(int density){

        SimObject=new GameObject("Simulation");
        //add the script to the gameobject
        sc=SimObject.AddComponent<SimulationConfigurer>();
        sc.VehicleDensity=SimGroups[density];
        sc.trafficSystem=findObjectOfType<TrafficSystem>();
        sm=SimObject.AddComponent<SimulationMaster_>();
        sm.sga=this;
        dh=SimObject.AddComponent<DataHandler>();
        dh.location=Location;
        dh.density=SimGroups[density];
        dh.isDataProcessed=false;
    }

    public void EndOfSimulation(){
        //TODO - Get data processor to process data and wait for complete

        //delay until data is processed
        while(!dh.isDataProcessed){
            //delay function for a second
        }

        // End of simulation
        // Destroy all the clones
        Destroy(dh.gameObject);
        Destroy(sc.gameObject);
        Destroy(sm.gameObject);

        // if there are more simulations to run
        if(group_index<SimGroups.Count){
            group_index++;
            StartSim(group_index);
        }
    }






void startSimIndepComp(){
            // Data Handler
        // DataHandler cloneDH=Instantiate(dh);
        // cloneDH.gameObject.SetActive(true);

        // Simulation Configurations
        // SimulationConfigurer cloneConfig=Instantiate(sc);
        // cloneConfig.VehicleDensity=SimGroups[density];
        // set any additional configurations here    
        // cloneConfig.gameObject.SetActive(true);

        // Simulation Master
        // SimulationMaster_ cloneSM=Instantiate(sm);
        // cloneSM.gameObject.SetActive(true);
        //create gameobject as child
}
}
