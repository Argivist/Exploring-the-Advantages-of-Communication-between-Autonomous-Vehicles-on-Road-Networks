using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;
using static SimulationConfigurer;

public class SimulationMaster_ : MonoBehaviour


{
public int limit=1000;
    [Header("Simulation Master")]

    public StopWatch sw;
    public SimGroupAutomate sga;

    public SimulationConfigurer sc;

    [Header("Data Handler")]
    public DataHandler dh;

    [Header("Vehicle Data List")]
    private List<Vehicle> vehicleList;
    private List<Vehicle> tempList;
    public GameObject Vehicle;

    [Header("Tracking Vehicles")]
    public int NumSpawnedVehicles;
    public int NumDestroyedVehicles;
    public bool nextSim = false;

    [Header("Current Simulation Index")]
    public int currSim = 0;
    private bool isReady = false;

    private void Start()
    {
        if (!ValidateComponents()) return;

        StartCoroutine(CheckConfigComplete());
    }

    private bool ValidateComponents()
    {
        if (SimConfig == null)
        {
            SimConfig = FindObjectOfType<SimConfig>();
            return false;
        }

        timer = GetComponent<Timer>();
        if (timer == null)
        {
            Debug.LogError("Timer component is missing. Please add it to the GameObject.");
            return false;
        }

        return true;
    }



    // Start is called before the first frame update
    // void Start()
    // {
    //     gameObject.AddComponent<StopWatch>();
    //     sw = gameObject.GetComponent<StopWatch>();
    //     sw.startTimer();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     dh.recordTime(Random.Range(0,4),sw.getTime(), Random.Range(0,100));
    //     if(sw.getTime()>limit){
    //         sw.stopTimer();
    //         dh.ProcessData();
    //         // wait for data to be processed
    //         if(sga.dh.isDataProcessed){
    //             sga.EndOfSimulation();
    //         }
            
    //     }
    // }
}
