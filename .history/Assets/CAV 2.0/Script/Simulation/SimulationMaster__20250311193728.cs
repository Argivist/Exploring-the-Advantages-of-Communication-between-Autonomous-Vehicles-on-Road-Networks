using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;
using static SimulationConfigurer;

public class SimulationMaster_ : MonoBehaviour


{
    public int limit = 1000;
    [Header("Simulation Master")]

    public StopWatch sw;
    public SimGroupAutomate sga;

    public SimulationConfigurer sc;

    public bool NormalSimulationEnabled = true;
    public bool CAVSimulationEnabled = true;
    public bool MixedSimulationEnabled = true;


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



    private void Update()
    {
        if (!isReady) return; // Wait until the configuration is ready

        if (SimulationCompleted() || nextSim)
        {
            PrepareForNextSimulation();
        }
        else
        {
            RunCurrentSimulation();
        }
    }


    private bool SimulationCompleted()
    {
        return (NumSpawnedVehicles >= sc.vehicleList.Count && NumDestroyedVehicles >= sc.vehicleList.Count);//|| nextSim;
    }

    private void PrepareForNextSimulation()
    {
        if (currSim >= 2) // End of all simulations
        {
            currSim = 0;
            nextSim = false;
            Debug.Log("End of all simulations.");
            sga.EndOfSimulation();
            //perform any post simulation logic here

        }
        else
        {

            Debug.Log("End of simulation " + currSim);
            Debug.Log("Preparing for next simulation...");
            currSim++;
            ResetSimulationState();
        }
    }


    private void ResetSimulationState()
    {

        NumSpawnedVehicles = 0;
        NumDestroyedVehicles = 0;
        sw.resetTimer();
        sw.startTimer();

        // Reinitialize tempList for the current simulation
        tempList = new List<Vehicle>(sc.vehicleList);

        nextSim = false;
    }


    public void VehicleDestroyed(string carname, int id, int time)
    {
        NumDestroyedVehicles++;
        dh.recordTime(id, time, Random.Range(0, 100));

        if (NumDestroyedVehicles == sc.vehicleList.Count)
        {
            Debug.Log("All vehicles destroyed. Experiment complete.");
        }
    }

    private void RunCurrentSimulation()
    {

        switch (currSim)
        {
            case 0: // Normal
                if (NormalSimulationEnabled)
                {
                    SpawnNormalVehicle();
                }
                else
                {
                    nextSim = true; // Skip if not enabled
                }
                break;

            case 1: // CAV
                if (CAVSimulationEnabled)
                {
                    SpawnCAVehicle();
                }
                else
                {
                    nextSim = true; // Skip if not enabled
                }
                break;

            case 2: // Mixed
                if (MixedSimulationEnabled)
                {
                    SpawnMixedVehicles();
                }
                else
                {
                    sw.resetTimer();
                    nextSim = true; // Skip if not enabled
                }
                break;
        
        }
    }

        private void SpawnNormalVehicle()
{
    for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            GameObject vehiclePrefab = Vehicle;
            tempList[i].vehicleType = Navigation.VehicleType.NonCAV;
            InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
            tempList.RemoveAt(i);
        }
    }
}

private void SpawnCAVehicle()
{
    for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            GameObject vehiclePrefab = Vehicle;
            tempList[i].vehicleType = Navigation.VehicleType.CAV;
            InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
            tempList.RemoveAt(i);
        }
    }
}

private void SpawnMixedVehicles()
{
    for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            InstantiateAndTrackVehicle(Vehicle, tempList[i]);
            tempList.RemoveAt(i);
        }
    }
}


    private void InstantiateAndTrackVehicle(GameObject prefab, Vehicle vehicleData)
    {
        GameObject vehicle = Instantiate(prefab, vehicleData.startPos, Quaternion.identity);
        vehicle.name=vehicleData.vehicleName;
        vehicle.GetComponent<VehicleSpawnerObject>().dest=vehicleData.endPos;
        vehicle.SetActive(true);
        vehicle.GetComponent<VehicleSpawnerObject>().WayDir = vehicleData.wdir;
        if(vehicleData.vehicleType == VehicleType.CAV)
        {
            vehicle.GetComponent<VehicleSpawnerObject>().type = Navigation.VehicleType.CAV;
        }
        else
        {
            vehicle.GetComponent<VehicleSpawnerObject>().type = Navigation.VehicleType.NonCAV;
        }
        vehicle.GetComponent<VehicleSpawnerObject>().destSegment = vehicleData.destSegment;
        // Uncomment and implement as needed:
        // vehicle.GetComponent<Vehicle>().SetVehicle(vehicleData.Speed, vehicleData.EndPos, vehicleData.StartTime, vehicleData.EndTime);
        // SpawnedVehicles.Add(vehicle);
        NumSpawnedVehicles++;
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
