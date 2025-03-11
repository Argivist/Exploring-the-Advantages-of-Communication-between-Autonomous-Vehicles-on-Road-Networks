using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using static Navigation;

public class SimulationMaster : MonoBehaviour
{


    public SimConfig SimConfig;
    public bool NormalSimulationEnabled;
    public bool CAVSimulationEnabled;
    public bool MixedSimulationEnabled;

    private Timer timer;
    public GameObject Vehicle;

    // Sim Configuration
    private List<SimConfig.Vehicle> vehicleList;
    private List<SimConfig.Vehicle> tempList;

    // Tracking vehicles in the simulation
    public int NumSpawnedVehicles;
    public int NumDestroyedVehicles;
    public bool nextSim = false;


    // Current simulation index
    public int currSim = 0;
    private bool isReady = false; // Indicates if the simulation is ready to start



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

    private IEnumerator CheckConfigComplete()
    {
        float timeout = 10f; // Maximum wait time in seconds
        float elapsedTime = 0f;

        while (!SimConfig.ready)
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

        if (SimConfig.vehicleList == null || SimConfig.vehicleList.Count == 0)
        {
            Debug.LogError("Vehicle list in SimConfig is empty or null.");
            yield break;
        }

        vehicleList = new List<SimConfig.Vehicle>(SimConfig.vehicleList);
        tempList = new List<SimConfig.Vehicle>(vehicleList);

        Debug.Log("Vehicle list initialized with " + vehicleList.Count + " vehicles.");

        // Start timer and mark readiness
        timer.StartTimer();
        isReady = true;
    }



    private void Update()
    {
        if (!isReady) return; // Wait until the configuration is ready

        if (SimulationCompleted())
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
        return (NumSpawnedVehicles >= SimConfig.vehicleList.Count && NumDestroyedVehicles >= SimConfig.vehicleList.Count);//|| nextSim;
    }




    private void PrepareForNextSimulation()
    {
        if (currSim >= 2) // End of all simulations
        {
            currSim = 0;
            nextSim = false;
            Debug.Log("End of all simulations.");
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

        // Reinitialize tempList for the current simulation
        tempList = new List<SimConfig.Vehicle>(SimConfig.vehicleList);

        nextSim = false;
    }

    public void VehicleDestroyed(string carname,int time){
        NumDestroyedVehicles++;
        //Log time

        if (NumDestroyedVehicles == SimConfig.vehicleList.Count)
        {
            Debug.Log("All vehicles destroyed. Experiment complete.");
            }
    }
// public void VehicleDataGatherer(string carname,int time)
//     {
        
//         //convert time to seconds and int
//         // int time = (int)Time.time;
//         // vehicleConfig.UpdateTTD(carname,"CAV",time);
//         // if (NumDestroyedVehicles == vehicleConfig.num_cars)

//         if (NumDestroyedVehicles == SimConfig.vehicleList.Count)
//         {
//             Debug.Log("All vehicles destroyed. Experiment complete.");
//             // Time.timeScale = 0; // Stop timer
//             //find and destroy this spawner
//             // Destroy(GameObject.Find(this.name));
//         }

//     }

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
            nextSim = true; // Skip if not enabled
        }
        break;
}
    
    }


    // private void SpawnVehicles(GameObject vehiclePrefab, VehicleType type)
    // {
    //     for (int i = tempList.Count - 1; i >= 0; i--)
    //     {
    //         if (tempList[i].startTime <= timer.GetTimer() && tempList[i].vehicleType == type)
    //         {
    //             Debug.Log($"Spawning vehicle of type {type} at {tempList[i].startPos}");
    //             InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
    //             tempList.RemoveAt(i);
    //         }
    //     }
    // }

    //Spawn all normal
    private void SpawnNormalVehicle()
    {
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            if (tempList[i].startTime <= timer.GetTimer())
            {

                GameObject vehiclePrefab = Vehicle;
                // vehiclePrefab.GetComponent<VehicleSpawnerObject>().type=Navigation.VehicleType.NonCAV;
                tempList[i].vehicleType == Navigation.VehicleType.NonCAV;


                InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);

                tempList.RemoveAt(i);
            }
        }
    }

    //spawn all cav
    private void SpawnCAVehicle()
    {
        for (int i = tempList.Count - 1; i >= 0; i--)
        {

            if (tempList[i].startTime <= timer.GetTimer())
            {

                GameObject vehiclePrefab = Vehicle;
                // tempList[i].vehicleType == SimConfig.VehicleType.Normal
                // ? NormalVehicle
                // : CAVehicle;

                InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
                tempList.RemoveAt(i);
            }
        }
    }
    // Spawn Mixed Vehicle
    private void SpawnMixedVehicles()
    {
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            if (tempList[i].startTime <= timer.GetTimer())
            {
                // GameObject vehiclePrefab = tempList[i].vehicleType == SimConfig.VehicleType.Normal
                //     ? NormalVehicle
                //     : CAVehicle;
            
                InstantiateAndTrackVehicle(Vehicle, tempList[i]);
                tempList.RemoveAt(i);
            }
        }
    }


    private void InstantiateAndTrackVehicle(GameObject prefab, SimConfig.Vehicle vehicleData)
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
}

// TODO:
// - Simulation loop based on the number of simulation types.
// - Activate/reset timer.
// - Configure and spawn vehicles:
//   - If only Normal: Enable Normal and configure vehicles.
//   - If Mixed: Enable Mixed with default values.
//   - If CAV: Enable CAV and configure vehicles.
//   - Else: Skip.
// - Spawn configured vehicles.
// - Wait for all vehicles to de-spawn.
// - Repeat.
