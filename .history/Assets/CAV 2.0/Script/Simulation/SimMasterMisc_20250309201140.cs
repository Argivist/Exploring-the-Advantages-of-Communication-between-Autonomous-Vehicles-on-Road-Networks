
// using System.Collections;
// using System.Collections.Generic;
// using JetBrains.Annotations;
// using UnityEngine;

// public class SimulationMaster : MonoBehaviour
// {
//     public enum SimOptions
//     {
//         Normal,
//         CAV,
//         Mixed
//     };

//     public SimConfig SimConfig;
//     public bool NormalSimulationEnabled;
//     public bool CAVSimulationEnabled;
//     public bool MixedSimulationEnabled;

//     private Timer timer;
//     public GameObject NormalVehicle, CAVehicle;


//     //Sim Configuration
//     List<SimConfig.Vehicle> vehicleList;
//     List<SimConfig.Vehicle> TempList;



//     //Tracking vehicles in the sim
//     public int NumSpawnedVehicles;
//     public int NumDestroyedVehicles;
//     public bool nextSim = false;
//     public List<GameObject> SpawnedVehicles;

//     //current simulation int
//     public int currSim = 0;

//     public void Start()
//     {

//         if (SimConfig == null)
//         {
//             Debug.LogError("SimConfig is not assigned. Please assign it in the Inspector or initialize it in code.");
//             return;
//         }

//         timer = GetComponent<Timer>();
//         if (timer == null)
//         {
//             Debug.LogError("Timer component is missing. Please add it to the GameObject.");
//             return;
//         }

//         StartCoroutine(CheckConfigComplete());

//     }

//     IEnumerator CheckConfigComplete()
//     {
//         float timeout = 10f; // Maximum wait time in seconds
//         float elapsedTime = 0f;

//         while (!SimConfig.ready)
//         {
//             Debug.Log("Waiting for config to be ready...");
//             yield return new WaitForSeconds(0.5f); // Wait 0.5 seconds before rechecking
//             elapsedTime += 0.5f;

//             if (elapsedTime >= timeout)
//             {
//                 Debug.LogError("Timed out waiting for config to be ready.");
//                 yield break;
//             }
//         }

//         Debug.Log("Config is ready.");


//         if (SimConfig.vehicleList == null)
//         {
//             Debug.LogError("Vehicle list is not initialized in SimConfig.");
//             yield break;
//         }

//         // Import Vehicle Information List
//         vehicleList = SimConfig.vehicleList;

//         // Start timer
//         timer.StartTimer();
//     }

//     public void Update()
//     {
//         if ((NumSpawnedVehicles >= vehicleList.Count && NumDestroyedVehicles >= vehicleList.Count) || nextSim)
//         {
//             if (currSim == 2)
//             {
//                 currSim = 0;
//                 nextSim = false;
//                 //end of all sims
//             }
//             else
//             {
//                 currSim++;
//                 NumSpawnedVehicles = 0;
//                 NumDestroyedVehicles = 0;
//                 timer.StopTimer();
//                 timer.ResetTimer();
//                 TempList = new List<SimConfig.Vehicle>(vehicleList);
//                 timer.StartTimer();
//                 nextSim = false;
//             }
//         }
//         else
//         {
//             if (currSim == 0)//All Normal
//             {
//                 if (NormalSimulationEnabled)
//                 {
//                     if (NumSpawnedVehicles < vehicleList.Count)
//                     {
//                         //goo through the list anmd spawn vehicles which have reached their start time and delete them from the list
//                         for (int i = TempList.Count; i >0 ; i--)
//                         {
//                             if (TempList[i].startTime <= timer.GetTimer())
//                             {
//                                 GameObject vehicle = Instantiate(NormalVehicle, TempList[i].startPos, Quaternion.identity);
//                                 // vehicle.GetComponent<Vehicle>().SetVehicle(TempList[i].Speed, TempList[i].EndPos, TempList[i].StartTime, TempList[i].EndTime);
//                                 SpawnedVehicles.Add(vehicle);
//                                 NumSpawnedVehicles++;
//                                 TempList.RemoveAt(i);
//                             }
//                         }
//                     }
//                 }
//                 else
//                 {
//                     nextSim = true;
//                 }
//             }
//             else if (currSim == 1)//All CAV
//             {
//                 if (CAVSimulationEnabled)
//                 {
//                     if (NumSpawnedVehicles < vehicleList.Count)
//                     {
//                         //goo through the list anmd spawn vehicles which have reached their start time and delete them from the list
//                         for (int i = TempList.Count; i >0 ; i--)
//                         {
//                             if (TempList[i].startTime <= timer.GetTimer())
//                             {
//                                 GameObject vehicle = Instantiate(CAVehicle, TempList[i].startPos, Quaternion.identity);
//                                 // vehicle.GetComponent<Vehicle>().SetVehicle(TempList[i].Speed, TempList[i].EndPos, TempList[i].StartTime, TempList[i].EndTime);
//                                 SpawnedVehicles.Add(vehicle);
//                                 NumSpawnedVehicles++;
//                                 TempList.RemoveAt(i);
//                             }
//                         }
//                     }
//                 }
//                 else
//                 {
//                     nextSim = true;
//                 }
//             }
//             else if (currSim == 2)//All Mixed
//             {
//                 if (MixedSimulationEnabled)
//                 {
//                     if (NumSpawnedVehicles < vehicleList.Count)
//                     {
//                         //goo through the list anmd spawn vehicles which have reached their start time and delete them from the list
//                         for (int i = TempList.Count; i >0 ; i--)
//                         {
//                             if (TempList[i].startTime <= timer.GetTimer())
//                             {
//                                 if (TempList[i].vehicleType == SimConfig.VehicleType.Normal)
//                                 {
//                                     GameObject vehicle = Instantiate(NormalVehicle, TempList[i].startPos, Quaternion.identity);
//                                     // vehicle.GetComponent<Vehicle>().SetVehicle(TempList[i].Speed, TempList[i].EndPos, TempList[i].StartTime, TempList[i].EndTime);
//                                     SpawnedVehicles.Add(vehicle);
//                                     NumSpawnedVehicles++;
//                                     TempList.RemoveAt(i);
//                                 }
//                                 else if (TempList[i].vehicleType == SimConfig.VehicleType.CAV)
//                                 {
//                                     GameObject vehicle = Instantiate(CAVehicle, TempList[i].startPos, Quaternion.identity);
//                                     // vehicle.GetComponent<Vehicle>().SetVehicle(TempList[i].Speed, TempList[i].EndPos, TempList[i].StartTime, TempList[i].EndTime);
//                                     SpawnedVehicles.Add(vehicle);
//                                     NumSpawnedVehicles++;
//                                     TempList.RemoveAt(i);
//                                 }
//                             }
//                         }
//                     }
//                 }
//                 else
//                 {
//                     nextSim = true;
//                 }
//             }
//             // if (currSim == 0)//All Normal
//             // {
//             //     if (NormalSimulationEnabled)
//             //     {
//             //         if (NumSpawnedVehicles < vehicleList.Count)
//             //         {
//             //         //get all vehicles in the list with start time less or equal to timer time and delete from the 
//             //         }
//             //     }
//             // }

//         }


//     }

// }


// //TODO - 

// // - simulation loop based on number of sim types
// //      - activate/reset timer
// //      - configure and spawn vehicle
// //              - If only normal i=[0] and normal enabled config to be only normal
// //              - iF MIXED i=[1] and mixed enabled configure as defasult values
// //              - If CAV -=[2] and CAV enabled configure cav
// //              - Else skip
// //                      - spawn configured vehiclesx  
// //      - wait for all vehicles to despawn
// //      - repeat



////////////////////////////////////////////////////////////////////////////!SECTION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationMasterMisc : MonoBehaviour
{
    public enum SimOptions
    {
        Normal,
        CAV,
        Mixed
    };

    public SimConfig SimConfig;
    public bool NormalSimulationEnabled;
    public bool CAVSimulationEnabled;
    public bool MixedSimulationEnabled;

    private Timer timer;
    public GameObject NormalVehicle, CAVehicle;

    // Sim Configuration
    private List<SimConfig.Vehicle> vehicleList;
    private List<SimConfig.Vehicle> tempList;

    // Tracking vehicles in the simulation
    public int NumSpawnedVehicles;
    public int NumDestroyedVehicles;
    public bool nextSim = false;
    public List<GameObject> SpawnedVehicles;

    // Current simulation index
    public int currSim = 0;

    private void Start()
    {
        if (!ValidateComponents()) return;

        StartCoroutine(CheckConfigComplete());
        
    }

    private bool ValidateComponents()
    {
        if (SimConfig == null)
        {
            Debug.LogError("SimConfig is not assigned. Please assign it in the Inspector or initialize it in code.");
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

    if (SimConfig.vehicleList == null)
    {
        Debug.LogError("Vehicle list is not initialized in SimConfig.");
        yield break;
    }

    // Initialize vehicleList as a new copy to avoid external modifications
    vehicleList = new List<SimConfig.Vehicle>(SimConfig.vehicleList);
    tempList = new List<SimConfig.Vehicle>(vehicleList);

    Debug.Log("Vehicle list initialized with " + vehicleList.Count + " vehicles.");

    // Start timer
    timer.StartTimer();
}


    private void Update()
    {
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
    if (vehicleList == null)
    {
        vehicleList = new List<SimConfig.Vehicle>();
        Debug.LogError("vehicleList is null in SimulationCompleted.");
        return false;
    }

    return (NumSpawnedVehicles >= vehicleList.Count && NumDestroyedVehicles >= vehicleList.Count) || nextSim;
}



    private void PrepareForNextSimulation()
    {
        if (currSim >= 2) // End of all simulations
        {
            currSim = 0;
            nextSim = false;
            Debug.Log("End of all simulations.");
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
    if (vehicleList == null)
    {
        Debug.LogError("vehicleList is null during ResetSimulationState.");
        return;
    }

    NumSpawnedVehicles = 0;
    NumDestroyedVehicles = 0;

    // Always create a new tempList from the original vehicleList
    tempList = new List<SimConfig.Vehicle>(vehicleList);

    Debug.Log("Simulation state reset. TempList contains " + tempList.Count + " vehicles.");

    timer.StopTimer();
    timer.ResetTimer();
    timer.StartTimer();

    nextSim = false;
}

    private void RunCurrentSimulation()
    {
        switch (currSim)
        {
            case 0: // All Normal
                if (NormalSimulationEnabled)
                    SpawnVehicles(NormalVehicle, SimConfig.VehicleType.Normal);
                else
                    nextSim = true;
                break;

            case 1: // All CAV
                if (CAVSimulationEnabled)
                    SpawnVehicles(CAVehicle, SimConfig.VehicleType.CAV);
                else
                    nextSim = true;
                break;

            case 2: // Mixed
                if (MixedSimulationEnabled)
                    SpawnMixedVehicles();
                else
                    nextSim = true;
                break;
        }
    }

    private void SpawnVehicles(GameObject vehiclePrefab, Navigation.VehicleType type)
    {
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            if (tempList[i].startTime <= timer.GetTimer() && tempList[i].vehicleType == type)
            {
                InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
                tempList.RemoveAt(i);
            }
        }
    }

    private void SpawnMixedVehicles()
    {
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            if (tempList[i].startTime <= timer.GetTimer())
            {
                GameObject vehiclePrefab = tempList[i].vehicleType == SimConfig.VehicleType.Normal
                    ? NormalVehicle
                    : CAVehicle;

                InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
                tempList.RemoveAt(i);
            }
        }
    }

    private void InstantiateAndTrackVehicle(GameObject prefab, SimConfig.Vehicle vehicleData)
    {
        GameObject vehicle = Instantiate(prefab, vehicleData.startPos, Quaternion.identity);
        // Uncomment and implement as needed:
        // vehicle.GetComponent<Vehicle>().SetVehicle(vehicleData.Speed, vehicleData.EndPos, vehicleData.StartTime, vehicleData.EndTime);
        SpawnedVehicles.Add(vehicle);
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
// - Wait for all vehicles to despawn.
// - Repeat.

