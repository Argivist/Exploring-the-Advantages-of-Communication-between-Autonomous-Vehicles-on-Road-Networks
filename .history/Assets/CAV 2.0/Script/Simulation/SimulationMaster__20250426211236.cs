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

    public Dictionary<int, GameObject> SpawnedVehicles = new Dictionary<int, GameObject>();


    [Header("Data Handler")]
    public DataHandler dh;

    [Header("Vehicle Data List")]
    private Dictionary<int, Vehicle> vehicleList;
    private Dictionary<int, Vehicle> tempList;
    public GameObject Vehicle;

    [Header("Tracking Vehicles")]
    public int NumSpawnedVehicles;
    public int NumDestroyedVehicles;
    public bool nextSim = false;

    [Header("Current Simulation Index")]
    public int currSim = 0;
    private bool isReady = false;
    private bool spawningVehicles = false;
    private bool changedSim;

    [Header("Communication System")]
    private CommunicationSystem communicationSystem;

    private void Start()
    {
        if (!ValidateComponents()) return;

        StartCoroutine(CheckConfigComplete());
        // sw.startTimer();
    }

    private bool ValidateComponents()
    {
        if (sc == null)
        {
            // Debug.Log("SimulationConfigurer is missing. Trying to find it in the scene...");
            // sc = FindObjectOfType<SimulationConfigurer>();
            // return false;
            sc = FindObjectOfType<SimulationConfigurer>();
        }
        if (communicationSystem == null)
        {
            communicationSystem = FindObjectOfType<CommunicationSystem>();
            if (communicationSystem == null)
            {
                Debug.LogError("CommunicationSystem is missing. Please add it to the scene.");
                return false;
            }
        }
        // gameObject.AddComponent<StopWatch>();
        sw = GetComponent<StopWatch>();
        if (sw == null)
        {
            Debug.LogError("Timer component is missing. Please add it to the GameObject.");
            return false;
        }

        return true;
    }
    private IEnumerator CheckConfigComplete()
    {
        Debug.Log("Checking if config is ready...");
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

        // vehicleList = new List<Vehicle>(sc.vehicleList);
        // vehicleList = new Dictionary<int,Vehicle>();
        vehicleList = new Dictionary<int, Vehicle>(sc.vehicleDictionary);
        // tempList = new List<Vehicle>(vehicleList);
        tempList = new Dictionary<int, Vehicle>();
        tempList = new Dictionary<int, Vehicle>(vehicleList);

        Debug.Log("Vehicle list initialized with " + vehicleList.Count + " vehicles. TempList initialized with " + tempList.Count + " vehicles.");

        // Start timer and mark readiness
        sw.startTimer();
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
        communicationSystem.ResetSystem();
        if (currSim >= 2) // End of all simulations
        {
            currSim = 0;
            nextSim = false;
            Debug.Log("End of all simulations.");
            sga.EndOfSimulation();
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

        sw.stopTimer();
        sw.resetTimer();
        sw.startTimer();
        Debug.Log("Timer reset and started for simulation " + currSim);
        Debug.Log("Generating TempList for simulation " + currSim);
        // sw.resetTimer();
        // sw.startTimer();

        // Reinitialize tempList for the current simulation
        // tempList = new List<Vehicle>(sc.vehicleList);
        tempList = new Dictionary<int, Vehicle>(sc.vehicleDictionary);
        Debug.Log("TempList reinitialized with " + tempList.Count + " vehicles. in sim"+ currSim);
        changedSim = true;
        nextSim = false;
    }


    public void VehicleDestroyed(int id, float time)
    {
        NumDestroyedVehicles++;
        dh.recordTime(id, time, currSim);
        Debug.Log("Vehicle " + id + " destroyed at time " + time + " in simulation " + currSim);

        if (NumDestroyedVehicles == sc.vehicleList.Count)
        {

            Debug.Log("All vehicles destroyed. Experiment complete.");
        }
    }

    private void RunCurrentSimulation()
    {
        // Original Spawn Logic
        if (!manual)
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
                    // sw.resetTimer();
                    nextSim = true; // Skip if not enabled
                }
                break;

        }}else{
        if (spawningVehicles && !changedSim) {
            Debug.Log("Spawning vehicles in progress. Skipping this frame.");
            return; // Skip if vehicles are already being spawned
        } 
        spawningVehicles = true;
        changedSim = false;
        Debug.Log("Running simulation " + currSim);
        switch (currSim)
        {
            case 0: // Normal
                if (NormalSimulationEnabled)
                {
                    StartCoroutine(SpawnNormalVehiclesCoroutine());
                }
                else
                {
                    nextSim = true;
                }
                break;

            case 1: // CAV
                if (CAVSimulationEnabled)
                {
                    StartCoroutine(SpawnCAVehiclesCoroutine());
                }
                else
                {
                    nextSim = true;
                }
                break;

            case 2: // Mixed
                if (MixedSimulationEnabled)
                {
                    StartCoroutine(SpawnMixedVehiclesCoroutine());
                }
                else
                {
                    nextSim = true;
                }
                break;
        
    }
        }

}

// Courutine to spawn vehicles based on the current simulation
private IEnumerator SpawnNormalVehiclesCoroutine()
{
    while (tempList.Count > 0)
    {
        var vehiclesToSpawn = new List<int>();
        List<int> removedVehicles = new List<int>();

        foreach (var kvp in tempList)
        {
            if (kvp.Value.startTime <= sw.getTime())
            {
                vehiclesToSpawn.Add(kvp.Key);
            }
        }

        foreach (var key in vehiclesToSpawn)
        {
            var vehicleData = tempList[key];
            vehicleData.vehicleType = Navigation.VehicleType.NonCAV;
            InstantiateAndTrackVehicle(Vehicle, vehicleData);
            removedVehicles.Add(key);
            // tempList.Remove(key);
            yield return null; // wait 1 frame after each spawn
        }

        foreach (var key in removedVehicles)
        {
            tempList.Remove(key);
        }


        spawningVehicles = false;

        yield return null;
    }
}

private IEnumerator SpawnCAVehiclesCoroutine()
{

    List<int> removedVehicles = new List<int>();
    while (tempList.Count > 0)
    {
        var vehiclesToSpawn = new List<int>();

        foreach (var kvp in tempList)
        {
            if (kvp.Value.startTime <= sw.getTime())
            {
                vehiclesToSpawn.Add(kvp.Key);
            }
        }

        foreach (var key in vehiclesToSpawn)
        {
            var vehicleData = tempList[key];
            vehicleData.vehicleType = Navigation.VehicleType.CAV;
            InstantiateAndTrackVehicle(Vehicle, vehicleData);
            // tempList.Remove(key);
            removedVehicles.Add(key);
            yield return null; // wait 1 frame after each spawn
        }

        foreach (var key in removedVehicles)
        {
            tempList.Remove(key);
        }
        spawningVehicles = false;

        yield return null;
    }
}

private IEnumerator SpawnMixedVehiclesCoroutine()
{

    List<int> removedVehicles = new List<int>();
    while (tempList.Count > 0)
    {
        var vehiclesToSpawn = new List<int>();

        foreach (var kvp in tempList)
        {
            if (kvp.Value.startTime <= sw.getTime())
            {
                vehiclesToSpawn.Add(kvp.Key);
            }
        }

        foreach (var key in vehiclesToSpawn)
        {
            var vehicleData = tempList[key];
            InstantiateAndTrackVehicle(Vehicle, vehicleData);
            // tempList.Remove(key);
            removedVehicles.Add(key);

            yield return null; // wait 1 frame after each spawn
        }

        foreach (var key in removedVehicles)
        {
            tempList.Remove(key);
        }
        spawningVehicles = false;

        yield return null;
    }
}

// Original spawn code
private void SpawnNormalVehicle()
{
    bool isSpawned = false;
    // for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    foreach (var i in tempList.Keys)
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            GameObject vehiclePrefab = Vehicle;
            tempList[i].vehicleType = Navigation.VehicleType.NonCAV;
            InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
            // tempList.RemoveAt(i);
            tempList.Remove(tempList[i].vehicleId);
            isSpawned = true;
        }
    }
}

private void SpawnCAVehicle()
{
    // for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    foreach (var i in tempList.Keys)
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            GameObject vehiclePrefab = Vehicle;
            tempList[i].vehicleType = Navigation.VehicleType.CAV;
            InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
            // tempList.RemoveAt(i);
            tempList.Remove(tempList[i].vehicleId);
        }
    }

}

private void SpawnMixedVehicles()
{
    // for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
    foreach (var i in tempList.Keys)
    {
        if (tempList[i].startTime <= sw.getTime())
        {
            InstantiateAndTrackVehicle(Vehicle, tempList[i]);
            // tempList.RemoveAt(i);
            tempList.Remove(tempList[i].vehicleId);
        }
    }
}


private void InstantiateAndTrackVehicle(GameObject prefab, Vehicle vehicleData)
{
    GameObject vehicle = Instantiate(prefab, vehicleData.startPos, Quaternion.identity);
    vehicle.name = vehicleData.vehicleName;
    vehicle.GetComponent<VehicleSpawnerObject>().dest = vehicleData.endPos;
    vehicle.SetActive(true);
    vehicle.GetComponent<VehicleSpawnerObject>().WayDir = vehicleData.wdir;
    vehicle.GetComponent<VehicleSpawnerObject>().dataHandler = dh;
    // Debug.LogWarning("Vehicle spawned: " + vehicleData.vehicleName+"faceing "+vehicleData.wdir);
    vehicle.GetComponent<VehicleSpawnerObject>().id = vehicleData.vehicleId;
    vehicle.GetComponent<VehicleSpawnerObject>().simNo = currSim;
    if (vehicleData.vehicleType == VehicleType.CAV)
    {
        vehicle.GetComponent<VehicleSpawnerObject>().type = VehicleType.CAV;
    }
    else
    {
        vehicle.GetComponent<VehicleSpawnerObject>().type = VehicleType.NonCAV;
    }
    vehicle.GetComponent<VehicleSpawnerObject>().destSegment = vehicleData.destSegment;
    // Uncomment and implement as needed:
    // vehicle.GetComponent<Vehicle>().SetVehicle(vehicleData.Speed, vehicleData.EndPos, vehicleData.StartTime, vehicleData.EndTime);
    // SpawnedVehicles.Add(vehicle);
    if (vehicle != null)
    {
        NumSpawnedVehicles++;
    }
    // SpawnedVehicles.Add(vehicle);
    // Check if the vehicle ID already exists in the dictionary
    if (!SpawnedVehicles.ContainsKey(vehicleData.vehicleId))
    {
        SpawnedVehicles.Add(vehicleData.vehicleId, vehicle);
    }
    else
    {
        Debug.LogWarning($"Vehicle with ID {vehicleData.vehicleId} already exists in SpawnedVehicles. Skipping adding again.");
    }
    // SpawnedVehicles.Add(vehicleData.vehicleId, vehicle);
}
}





// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using static Navigation;
// using static SimulationConfigurer;

// public class SimulationMaster_ : MonoBehaviour


// {
//     public int limit = 1000;
//     [Header("Simulation Master")]

//     public StopWatch sw;
//     public SimGroupAutomate sga;

//     public SimulationConfigurer sc;

//     public bool NormalSimulationEnabled = true;
//     public bool CAVSimulationEnabled = true;
//     public bool MixedSimulationEnabled = true;

//     public List<GameObject> SpawnedVehicles = new List<GameObject>();


//     [Header("Data Handler")]
//     public DataHandler dh;

//     [Header("Vehicle Data List")]
//     private List<Vehicle> vehicleList;
//     private List<Vehicle> tempList;
//     public GameObject Vehicle;

//     [Header("Tracking Vehicles")]
//     public int NumSpawnedVehicles;
//     public int NumDestroyedVehicles;
//     public bool nextSim = false;

//     [Header("Current Simulation Index")]
//     public int currSim = 0;
//     private bool isReady = false;

//     private void Start()
//     {
//         if (!ValidateComponents()) return;

//         StartCoroutine(CheckConfigComplete());
//         // sw.startTimer();
//     }

//     private bool ValidateComponents()
//     {
//         if (sc == null)
//         {
//             // Debug.Log("SimulationConfigurer is missing. Trying to find it in the scene...");
//             // sc = FindObjectOfType<SimulationConfigurer>();
//             // return false;
//             sc = FindObjectOfType<SimulationConfigurer>();
//         }
//         // gameObject.AddComponent<StopWatch>();
//         sw = GetComponent<StopWatch>();
//         if (sw == null)
//         {
//             Debug.LogError("Timer component is missing. Please add it to the GameObject.");
//             return false;
//         }

//         return true;
//     }
//     private IEnumerator CheckConfigComplete()
//     {
//         Debug.Log("Checking if config is ready...");
//         float timeout = 10f; // Maximum wait time in seconds
//         float elapsedTime = 0f;

//         while (!sc.ready)
//         {
//             Debug.Log("Waiting for config to be ready...");
//             yield return new WaitForSeconds(0.5f);
//             elapsedTime += 0.5f;

//             if (elapsedTime >= timeout)
//             {
//                 Debug.LogError("Timed out waiting for config to be ready.");
//                 yield break;
//             }
//         }

//         Debug.Log("Config is ready.");

//         if (sc.vehicleList == null || sc.vehicleList.Count == 0)
//         {
//             Debug.LogError("Vehicle list in SimConfig is empty or null.");
//             yield break;
//         }

//         vehicleList = new List<Vehicle>(sc.vehicleList);
//         tempList = new List<Vehicle>(vehicleList);

//         Debug.Log("Vehicle list initialized with " + vehicleList.Count + " vehicles. TempList initialized with " + tempList.Count + " vehicles.");

//         // Start timer and mark readiness
//         sw.startTimer();
//         isReady = true;
//     }



//     private void Update()
//     {
//         if (!isReady) return; // Wait until the configuration is ready

//         if (SimulationCompleted() || nextSim)
//         {
//             PrepareForNextSimulation();
//         }
//         else
//         {
//             RunCurrentSimulation();
//         }
//     }


//     private bool SimulationCompleted()
//     {
//         return (NumSpawnedVehicles >= sc.vehicleList.Count && NumDestroyedVehicles >= sc.vehicleList.Count);//|| nextSim;
//     }

//     private void PrepareForNextSimulation()
//     {
//         if (currSim >= 2) // End of all simulations
//         {
//             currSim = 0;
//             nextSim = false;
//             Debug.Log("End of all simulations.");
//             sga.EndOfSimulation();


//         }
//         else
//         {

//             Debug.Log("End of simulation " + currSim);
//             Debug.Log("Preparing for next simulation...");
//             currSim++;
//             ResetSimulationState();
//         }
//     }


//     private void ResetSimulationState()
//     {

//         NumSpawnedVehicles = 0;
//         NumDestroyedVehicles = 0;
//         sw.stopTimer();
//         sw.resetTimer();
//         sw.startTimer();
//         // sw.resetTimer();
//         // sw.startTimer();

//         // Reinitialize tempList for the current simulation
//         tempList = new List<Vehicle>(sc.vehicleList);

//         nextSim = false;
//     }


//     public void VehicleDestroyed(int id, float time)
//     {
//         NumDestroyedVehicles++;
//         dh.recordTime(id, time, currSim);
//         Debug.Log("Vehicle " + id + " destroyed at time " + time + " in simulation " + currSim);

//         if (NumDestroyedVehicles == sc.vehicleList.Count)
//         {

//             Debug.Log("All vehicles destroyed. Experiment complete.");
//         }
//     }

//     private void RunCurrentSimulation()
//     {

//         switch (currSim)
//         {
//             case 0: // Normal
//                 if (NormalSimulationEnabled)
//                 {
//                     SpawnNormalVehicle();
//                 }
//                 else
//                 {
//                     nextSim = true; // Skip if not enabled
//                 }
//                 break;

//             case 1: // CAV
//                 if (CAVSimulationEnabled)
//                 {
//                     SpawnCAVehicle();
//                 }
//                 else
//                 {
//                     nextSim = true; // Skip if not enabled
//                 }
//                 break;

//             case 2: // Mixed
//                 if (MixedSimulationEnabled)
//                 {
//                     SpawnMixedVehicles();
//                 }
//                 else
//                 {
//                     // sw.resetTimer();
//                     nextSim = true; // Skip if not enabled
//                 }
//                 break;

//         }
//     }

//     private void SpawnNormalVehicle()
//     {
//         bool isSpawned = false;
//         for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
//         {
//             if (tempList[i].startTime <= sw.getTime())
//             {
//                 GameObject vehiclePrefab = Vehicle;
//                 tempList[i].vehicleType = Navigation.VehicleType.NonCAV;
//                 InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
//                 tempList.RemoveAt(i);
//                 isSpawned = true;
//             }
//         }
//     }

//     private void SpawnCAVehicle()
//     {
//         for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
//         {
//             if (tempList[i].startTime <= sw.getTime())
//             {
//                 GameObject vehiclePrefab = Vehicle;
//                 tempList[i].vehicleType = Navigation.VehicleType.CAV;
//                 InstantiateAndTrackVehicle(vehiclePrefab, tempList[i]);
//                 tempList.RemoveAt(i);
//             }
//         }
//     }

//     private void SpawnMixedVehicles()
//     {
//         for (int i = tempList.Count - 1; i >= 0; i--) // Fix: i >= 0 instead of i > 0
//         {
//             if (tempList[i].startTime <= sw.getTime())
//             {
//                 InstantiateAndTrackVehicle(Vehicle, tempList[i]);
//                 tempList.RemoveAt(i);
//             }
//         }
//     }


//     private void InstantiateAndTrackVehicle(GameObject prefab, Vehicle vehicleData)
//     {
//         GameObject vehicle = Instantiate(prefab, vehicleData.startPos, Quaternion.identity);
//         vehicle.name = vehicleData.vehicleName;
//         vehicle.GetComponent<VehicleSpawnerObject>().dest = vehicleData.endPos;
//         vehicle.SetActive(true);
//         vehicle.GetComponent<VehicleSpawnerObject>().WayDir = vehicleData.wdir;
//         vehicle.GetComponent<VehicleSpawnerObject>().dataHandler = dh;
//         // Debug.LogWarning("Vehicle spawned: " + vehicleData.vehicleName+"faceing "+vehicleData.wdir);
//         vehicle.GetComponent<VehicleSpawnerObject>().id = vehicleData.vehicleId;
//         vehicle.GetComponent<VehicleSpawnerObject>().simNo = currSim;
//         if (vehicleData.vehicleType == VehicleType.CAV)
//         {
//             vehicle.GetComponent<VehicleSpawnerObject>().type = VehicleType.CAV;
//         }
//         else
//         {
//             vehicle.GetComponent<VehicleSpawnerObject>().type = VehicleType.NonCAV;
//         }
//         vehicle.GetComponent<VehicleSpawnerObject>().destSegment = vehicleData.destSegment;
//         // Uncomment and implement as needed:
//         // vehicle.GetComponent<Vehicle>().SetVehicle(vehicleData.Speed, vehicleData.EndPos, vehicleData.StartTime, vehicleData.EndTime);
//         // SpawnedVehicles.Add(vehicle);
//         if (vehicle != null)
//         {
//             NumSpawnedVehicles++;
//         }
//         SpawnedVehicles.Add(vehicle);
//     }
// }