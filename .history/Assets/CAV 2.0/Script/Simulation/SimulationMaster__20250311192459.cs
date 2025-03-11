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
        if (sc == null)
        {
            sc = FindObjectOfType<SimulationConfigurer>();
            return false;
        }
        
        return true;
    }
    private IEnumerator CheckConfigComplete()
    {
        float timeout = 10f; // Maximum wait time in seconds
        float elapsedTime = 0f;

        while (!sc.ready)
        {
            Debug.Log("Waiting for config to be ready...");
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;

            if (elapsedTime >= timeout)
            {
                Debug.LogError("Timed out waiting for config to be ready.");
                yield break;
            }
        }

        Debug.Log("Config is ready.");

        if (sc.vehicleList == null || sc.vehicleList.Count == 0)
        {
            Debug.LogError("Vehicle list in SimConfig is empty or null.");
            yield break;
        }

        vehicleList = new List<Vehicle>(sc.vehicleList);
        tempList = new List<Vehicle>(vehicleList);

        Debug.Log("Vehicle list initialized with " + vehicleList.Count + " vehicles.");

        // Start timer and mark readiness
        // timer.StartTimer();
        isReady = true;
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
