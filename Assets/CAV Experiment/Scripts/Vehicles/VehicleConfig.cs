using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class VehicleConfig : MonoBehaviour
{
    //waypoint with segmen, waypoiint adn waypoint position
    public class WaypointObject
    {
        public Segment segment;
        public Waypoint currentWaypoint;
        public Waypoint nextWaypoint;
        public Waypoint previousWaypoint;
        public Vector3 position;

        public WaypointObject(Segment segment, Waypoint current, Waypoint next, Waypoint previous, Vector3 position)
        {
            this.segment = segment;
            this.currentWaypoint = current;
            this.nextWaypoint = next;
            this.previousWaypoint = previous;
            this.position = position;
        }

    }

    public TrafficSystem trafficSystem;

    public bool ready = false;

    [System.Serializable]
    public class CarInfo
    {
        public string carName;
        public string carType;
        public string carColor;

        public int starttime;

        public int ttd_n; // time to destination normal sim

        public int ttd_cav; // time to destination CAV sim

        public Vector3 startPos;

        public Vector3 endPos;

        public CarInfo(string carName, string carType, string carColor, int spawnTime, Vector3 startPos, Vector3 endPos, int ttd_n = 0, int ttd_cav = 0)
        {
            this.carName = carName;
            this.carType = carType;
            this.carColor = carColor;
            this.starttime = spawnTime;
            this.startPos = startPos;
            this.endPos = endPos;
            this.ttd_n = ttd_n;
            this.ttd_cav = ttd_cav;
        }
    }
    //info for vehicle spawner: vehicle id, vehicle_type, start time, start point and end point
    [System.Serializable]
    public class VehicleInfo
    {
        public string vehicle_id;
        public string vehicle_type;
        public int start_time;
        public Vector3 start_point;
        public Vector3 end_point;

        public WaypointObject start_waypoint;
        public Segment start_segment;
        public Waypoint desination_waypoint;
        public Segment destination_segment;



        public VehicleInfo(string vehicle_id, string vehicle_type, int start_time, Vector3 start_point, Vector3 end_point, Waypoint desination_waypoint, Segment destination_segment, WaypointObject start_waypoint, Segment start_segment)
        {
            this.vehicle_id = vehicle_id;
            this.vehicle_type = vehicle_type;
            this.start_time = start_time;
            this.start_point = start_point;
            this.end_point = end_point;
            this.desination_waypoint = desination_waypoint;
            this.destination_segment = destination_segment;
            this.start_waypoint = start_waypoint;
            this.start_segment = start_segment;
        }
        
    }

    public List<CarInfo> carInfoList = new List<CarInfo>(); // List of car info objects
    public List<VehicleInfo> vehicleInfoList = new List<VehicleInfo>(); // List of vehicle info objects for spawner

    public int num_cars = 10; // Number of cars

    public GameObject vehicleSpawnerPrefab;

    public GameObject trafficcarprefab;

    void Start()
    {
        StartCoroutine(VconfStart());
    }

    IEnumerator VconfStart(){
            // Get all available waypoints in a list
        List<WaypointObject> waypointList = new List<WaypointObject>();
        foreach (Segment segment in trafficSystem.segments)
        {
            for (int i = 0; i < segment.waypoints.Count; i++)
            {
                Waypoint waypoint = segment.waypoints[i];
                Waypoint nextWaypoint = i < segment.waypoints.Count - 1 ? segment.waypoints[i + 1] : null;
                Waypoint previousWaypoint = nextWaypoint == null ? segment.waypoints[i - 1] : null;
                //Waypoint previousWaypoint = i > 0 ? segment.waypoints[i - 1] : null;
                waypointList.Add(new WaypointObject(segment, waypoint, nextWaypoint,previousWaypoint, waypoint.transform.position));
            }
        }

        // Initialize car info objects
        for (int i = 0; i < num_cars; i++)
        {
            // Random start time
            int start_time = Random.Range(0, 100);

            // Random waypoint selection
            int start_waypoint = Random.Range(0, waypointList.Count);
            // Random end waypoint selection not the same as start waypoint
            int end_waypoint = Random.Range(0, waypointList.Count);
            while (end_waypoint == start_waypoint)
            {
                end_waypoint = Random.Range(0, waypointList.Count);
            }


            // Ensure start and end waypoints are different
            while (end_waypoint == start_waypoint)
            {
                end_waypoint = Random.Range(0, waypointList.Count);
            }

            Vector3 startPos = waypointList[start_waypoint].position;
            Vector3 endPos = waypointList[end_waypoint].position;

            CarInfo carInfo = new CarInfo(
                "car" + i,
                "CAV",
                "color" + i,
                start_time,
                startPos,
                endPos,
                0,
                0
            );
            carInfoList.Add(carInfo);
            //add information for vehicle spawner	
            VehicleInfo vehicleInfo = new VehicleInfo(
                "car" + i,
                "type" + i,
                start_time,
                startPos,
                endPos,
                waypointList[end_waypoint].currentWaypoint,
                waypointList[end_waypoint].segment,
                waypointList[start_waypoint],
                waypointList[start_waypoint].segment
            );
            vehicleInfoList.Add(vehicleInfo);

            //inform the vehicle spawner that vehicle setup is done
            ready = true;

        }


        //save to excel
        // SavetoCSVcarInfo(carInfoList);

        //spawn vehicle spawner with normal type
        GameObject vehicleSpawner = Instantiate(vehicleSpawnerPrefab, Vector3.zero, Quaternion.identity);
        vehicleSpawner.GetComponent<V_Spawner>().experimentType = ExperimentType.Normal;
        vehicleSpawner.GetComponent<V_Spawner>().vehicleConfig = this;
        // vehicleSpawner.GetComponent<V_Spawner>().vehiclePrefab = trafficcarprefab;

        //wait for vehicle spawner normal to be destroyed
        while (vehicleSpawner != null)
        {
            yield return null;
        }

        //spawn vehicle spawner with CAV type
        vehicleSpawner = Instantiate(vehicleSpawnerPrefab, Vector3.zero, Quaternion.identity);
        vehicleSpawner.GetComponent<V_Spawner>().experimentType = ExperimentType.CAV;
        vehicleSpawner.GetComponent<V_Spawner>().vehicleConfig = this;
        // vehicleSpawner.GetComponent<V_Spawner>().vehiclePrefab = trafficcarprefab;

        //wait for vehicle spawner CAV to be destroyed
        while (vehicleSpawner != null)
        {
            yield return null;
        }

        //store the car info to excel
        SavetoCSVcarInfo(carInfoList);


    }

    void Update()
    {
        //spawn vehiicle spawner with normal type



        // Example update logic, e.g., track time to destination
        foreach (CarInfo car in carInfoList)
        {
            // car.ttd = Mathf.Max(0, car.ttd - 1); // Placeholder decrement logic
        }
    }

    void SavetoCSVcarInfo(List<CarInfo> carInfoList)
    {
        //save to excel
        string path = Application.dataPath + "/carInfo.csv";
        string delimiter = ",";

        List<string[]> output = new List<string[]>();
        output.Add(new string[] { "carName", "carType", "carColor", "starttime", "startPos", "endPos", "ttd_Normal", "ttd_CAV" });

        foreach (CarInfo car in carInfoList)
        {
            string start_pos = car.startPos.x.ToString() + "|" + car.startPos.y.ToString() + "|" + car.startPos.z.ToString();
            string end_pos = car.endPos.x.ToString() + "|" + car.endPos.y.ToString() + "|" + car.endPos.z.ToString();
            output.Add(new string[] { car.carName, car.carType, car.carColor, car.starttime.ToString(), start_pos, end_pos, car.ttd_n.ToString(), car.ttd_cav.ToString() });
        }

        int length = output.Count;

        string[][] outputArr = output.ToArray();

        string outPut = "";

        for (int i = 0; i < length; i++)
        {
            outPut += string.Join(delimiter, outputArr[i]) + "\n";
        }

        System.IO.File.WriteAllText(path, outPut);
    }


    // update ttd based on the car name and car type
    public void UpdateTTD(string carName, string carType, int ttd)
    {
        foreach (CarInfo car in carInfoList)
        {
            if (car.carName == carName)
            {
                if (car.carType == "CAV")
                {
                    car.ttd_cav = ttd;
                }
                else
                {
                    car.ttd_n = ttd;
                }
            }
        }
    }


}
