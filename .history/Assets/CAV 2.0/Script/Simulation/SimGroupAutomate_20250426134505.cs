using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

public class SimGroupAutomate : MonoBehaviour
{

    [Header("Simulation Groups")]
    public string Location;
    public List<int> SimGroups;

    [HideInInspector]
    public SimulationConfigurer sc;
    public SimulationMaster_ sm;

    public DataHandler dh;
    public StopWatch sw;
    public bool defaultSim=true;

    [Header("Config Type")]
    public bool ManualStartDestinationSetup=false;

    public Waypoint StartPosition;
    public Waypoint EndPosition;


    GameObject SimObject;//Object for handling simulation


    public int group_index=0;

    [Header("Vehicle")]
    public GameObject Vehicle;

    void Start(){
        
        StartSim(group_index);
    }


    void StartSim(int density){

        SimObject=new GameObject("Simulation");
        //add the script to the gameobject
        sw=SimObject.AddComponent<StopWatch>();
        sc=SimObject.AddComponent<SimulationConfigurer>();
        sc.VehicleDensity=SimGroups[density];
        sc.trafficSystem=FindObjectOfType<TrafficSystem>();
        sm=SimObject.AddComponent<SimulationMaster_>();
        sc.default_=defaultSim;
        sm.Vehicle=Vehicle;
        sm.sw=sw;
        sm.sga=this;
        dh=SimObject.AddComponent<DataHandler>();
        dh.location=Location;
        dh.density=SimGroups[density];
        dh.isDataProcessed=false;
        sc.dataHandler=dh;
        sm.dh=dh;
    }

    public void EndOfSimulation(){
        //TODO - Get data processor to process data and wait for complete
        dh.ProcessData();
        //delay until data is processed
        // while(!dh.isDataProcessed){
        //     //delay function for a second
        // }
        StartCoroutine(WaitForDataProcessing());
        // End of simulation
        // Destroy all the clones
        Destroy(SimObject);

        // if there are more simulations to run
        if(group_index<SimGroups.Count-1){
            group_index++;
            StartSim(group_index);
        }
    }
    IEnumerator WaitForDataProcessing()
{
    while (!dh.isDataProcessed)
    {
        yield return new WaitForSeconds(1f);
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
