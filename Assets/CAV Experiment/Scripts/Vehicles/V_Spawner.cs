using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

// experiment type, normal or CAV
public enum ExperimentType
{
    Normal,
    CAV
}


public class V_Spawner : MonoBehaviour
{
    public ExperimentType experimentType;
    public VehicleConfig vehicleConfig;
    public GameObject vehiclePrefab;

    int destroyedvehilcestotal = 0;

    void Start()
    {
        Time.timeScale = 1; // Enable timer
        StartCoroutine(SpawnVehicles());
    }

    IEnumerator SpawnVehicles()
    {
        while (vehicleConfig == null || !vehicleConfig.ready)
        {
            Debug.Log("Waiting for VehicleConfig to be ready...");
            yield return null;
        }

        Debug.Log("VehicleConfig is ready. Spawning vehicles...");
        List<VehicleConfig.VehicleInfo> vehicleInfoList = vehicleConfig.vehicleInfoList;
        vehicleInfoList.Sort((x, y) => x.start_time.CompareTo(y.start_time));

        foreach (VehicleConfig.VehicleInfo vehicleInfo in vehicleInfoList)
        {
            if (vehicleInfo.start_time < 0)
            {
                Debug.LogWarning($"Invalid start time for vehicle {vehicleInfo.vehicle_id}. Skipping...");
                continue;
            }

            if (vehicleInfo.start_segment == null || vehicleInfo.destination_segment == null)
            {
                Debug.LogError($"Vehicle {vehicleInfo.vehicle_id} has null segments. Skipping...");
                continue;
            }

            Vector3 currentWaypoint = vehicleInfo.start_waypoint.position;
            Vector3 nextWaypoint = vehicleInfo.start_waypoint.nextWaypoint?.transform.position ?? currentWaypoint + Vector3.forward;
            Vector3 orientation = (nextWaypoint - currentWaypoint).normalized;

            yield return new WaitForSeconds(vehicleInfo.start_time);

            GameObject vehicle = Instantiate(vehiclePrefab, vehicleInfo.start_point, Quaternion.LookRotation(orientation));
            vehicle.name = vehicleInfo.vehicle_id;



            Navigator navigator = vehicle.GetComponent<Navigator>();
            if (navigator != null)
            {
                navigator.currentSegment = vehicleInfo.start_segment;
                navigator.destinationSegment = vehicleInfo.destination_segment;
                Debug.Log($"Vehicle {vehicle.name} assigned segments: Start = {vehicleInfo.start_segment}, Destination = {vehicleInfo.destination_segment}");
            }
            else
            {
                Debug.LogError($"Navigator component missing in {vehicle.name}");
            }

            
        }
    }

    //update config ttd
    public void UpdateConfig(string carname)
    {
        destroyedvehilcestotal++;
        //convert time to seconds and int
        int time = (int)Time.time;
        vehicleConfig.UpdateTTD(carname,"CAV",time);

        if (destroyedvehilcestotal == vehicleConfig.num_cars)
        {
            Debug.Log("All vehicles destroyed. Experiment complete.");
            Time.timeScale = 0; // Stop timer
            //find and destroy this spawner
            Destroy(GameObject.Find(this.name));
        }

    }
    
    void Update()
    {
        // Additional logic for vehicles can go here
    }
}
