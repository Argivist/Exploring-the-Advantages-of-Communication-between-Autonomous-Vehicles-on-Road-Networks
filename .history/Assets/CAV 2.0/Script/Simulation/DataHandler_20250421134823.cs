using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;
using System.IO;

public class VehicleData
{
    int id;
    List<float> time_spent=new List<float>();
    List<int> simulation=new List<int>();

    Vector3 StartPosition;
    Vector3 EndPosition;
    VehicleType type;
    public VehicleData(int id, Vector3 StartPosition, Vector3 EndPosition, VehicleType type)
    {
        this.id = id;
        this.StartPosition = StartPosition;
        this.EndPosition = EndPosition;
        this.type = type;
    }

    public void recordTime(float time, int simulation)
    {
        time_spent.Add(time);
        this.simulation.Add(simulation);
    }
    public int getId()
    {
        return id;
    }

    public List<string[]> getData()
    {
        List<string[]> data = new List<string[]>();
        if (time_spent.Count == 0)
        {
            data.Add(new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), "0", "0","0","0","0" });	
        }else{
            
            string[] v=new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), simulation[0].ToString() };
            string[] times=new string[3];
            for(int x=0;x<3;x++){
                if(x<time_spent.Count){
                    times[x]=time_spent[x].ToString();
                }else{
                    times[x]="0";
                }
            }
            string[] joined = new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), simulation[0].ToString(), times[0], times[1], times[2] };
            data.Add(joined);
    
        }
        return data;


        // List<string[]> data = new List<string[]>();
        // if (time_spent.Count == 0)
        // {
        //     data.Add(new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), "0", "0" });
        // }else{
        // for (int i = 0; i < time_spent.Count; i++)
        // {
        //     data.Add(new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), time_spent[i].ToString(), simulation[i].ToString() });
        // }
        // }
        // return data;
    }

}
/////////////JSON Stuff///////////
public class vehicles_js{
    public List<vehicle_js> vehicles=new List<vehicle_js>();  

    public void addVehilcle(vehicle_js v){
        vehicles.Add(v);
    }  
}

public class vehicle_js{
    public int id;
    public List<path_js> path_norm=new List<path_js>();
    public List<path_js> path_cav_init=new List<path_js>();
    public List<path_js> path_cav_end=new List<path_js>();

    public vehicle_js(int id){
        this.id=id;
    }

    public void normalPath(path_js p){
        path_norm.Add(p);
    }
    public void cavPathInit(path_js p){
        path_cav_init.Add(p);
    }
    public void cavPathEnd(path_js p){
        path_cav_end.Add(p);
    }
}

public class path_js{
    public List<segment_track_js> path=new List<segment_track_js>();
    public List<segment_track_js> path_alt=new List<segment_track_js>();

    public void addSegment(segment_track_js s){
        path.Add(s);
    }
    public void addSegmentAlt(segment_track_js s, segment_track_js s2){
        path.Add(s);
        path_alt.Add(s2);
    }


}

public class segment_track_js{
    public int id;
    public float init_cost;
    public float end_cost;
    public float init_density;
    public float end_density;

    public segment_track_js(int id, float init_cost, float end_cost, float init_density, float end_density){
        this.id=id;
        this.init_cost=init_cost;
        this.init_density=init_density;
    }

    public void updateEndCost(float cost){
        init_cost=cost;
    }
    public void updateEndDensity(float density){
        end_density=density;
    }
}

//////////////////////////////////

public class DataHandler : MonoBehaviour
{
    [Header("Data Handler")]
    public string location;
    public int density;
    public bool isDataProcessed = false;
    [Header("Vehicle Data")]
    public List<VehicleData> vehicleList=new List<VehicleData>();
    public List<vehicle_js> vehicleList_js=new List<vehicle_js>();


    //called bu sim configurer to add vehicle data
    public void AddVehicleData(int id, Vector3 StartPosition, Vector3 EndPosition, VehicleType type)
    {
        vehicleList.Add(new VehicleData(id, StartPosition, EndPosition, type));
        vehicle_js v=new vehicle_js(id);

    }

    // sim master calls this function to record time spent by vehicle when vehicle reaches destination
    public void recordTime(int id, float time, int simulation)
    {
        foreach (VehicleData v in vehicleList)
        {
            if (v.getId() == id)
            {
                v.recordTime(time, simulation);
            }
        }
    }

    // sim master calls this function to process data when simulation is over
    public void ProcessData()
    {
                Debug.Log("Processing Data...");
        List<string[]> data = new List<string[]>();
        data.Add(new string[] { "ID", "Start Position X","Start Position Y","Start Position Z", "End Position X","End Position Y","End Position Z", "Type", "Simulation", "Time Spent_Norm","Time Spent_CAV","Time Spent_Mixed" });
        
        foreach (VehicleData v in vehicleList)
        {
            string[] s=new string[6];
            data.AddRange(v.getData());
        }
        SaveToCSV(data, Application.dataPath + "/SimulationData/" + location+"_"+density + ".csv");
        isDataProcessed = true;

        // save JSON
        


        // Debug.Log("Processing Data...");
        // List<string[]> data = new List<string[]>();
        // data.Add(new string[] { "ID", "Start Position X","Start Position Y","Start Position Z", "End Position X","End Position Y","End Position Z", "Type", "Time Spent", "Simulation" });
        // foreach (VehicleData v in vehicleList)
        // {
        //     data.AddRange(v.getData());
        // }
        // SaveToCSV(data, Application.dataPath + "/SimulationData/" + location+"_"+density + ".csv");
        // isDataProcessed = true;
    }

    private void SaveToCSV(List<string[]> data, string filename) 
{
    string directory = Application.dataPath + "/SimulationData/";


    // Ensure the directory exists
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }

    // Create full file path
    string filePath = Path.Combine(directory, filename);

    using (StreamWriter writer = new StreamWriter(filePath))
    {
        foreach (var row in data)
        {
            writer.WriteLine(string.Join(",", row)); // Write each row as CSV
        }
    }

    Debug.Log("CSV saved at: " + filePath);
}
private void SavetoJSON(List<vehicles_js> data, string filename) 
{
    string directory = Application.dataPath + "/SimulationData/";

    // Ensure the directory exists
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }

    // Create full file path
    string filePath = Path.Combine(directory, filename);

    using (StreamWriter writer = new StreamWriter(filePath))
    {
        foreach (var row in data)
        {
            writer.WriteLine(JsonUtility.ToJson(row)); // Write each row as JSON

}
    }

    Debug.Log("JSON saved at: " + filePath);
}

}