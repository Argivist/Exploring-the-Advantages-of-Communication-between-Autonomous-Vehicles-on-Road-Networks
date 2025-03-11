using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;
using System.IO;

public class VehicleData
{
    int id;
    List<int> time_spent;
    List<int> simulation;

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

    public void recordTime(int time, int simulation)
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

        for (int i = 0; i < time_spent.Count; i++)
        {
            data.Add(new string[] { id.ToString(), StartPosition.ToString(), EndPosition.ToString(), type.ToString(), time_spent[i].ToString(), simulation[i].ToString() });
        }
        return data;
    }

}

public class DataHandler : MonoBehaviour
{
    public List<VehicleData> vehicleList=new List<VehicleData>();
    [Header("Data Handler")]
    public string location;
    public int density;
    public bool isDataProcessed = true;


    //called bu sim configurer to add vehicle data
    public void AddVehicleData(int id, Vector3 StartPosition, Vector3 EndPosition, VehicleType type)
    {
        vehicleList.Add(new VehicleData(id, StartPosition, EndPosition, type));
    }

    // sim master calls this function to record time spent by vehicle when vehicle reaches destination
    public void recordTime(int id, int time, int simulation)
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
        List<string[]> data = new List<string[]>();
        data.Add(new string[] { "ID", "Start Position", "End Position", "Type", "Time Spent", "Simulation" });
        foreach (VehicleData v in vehicleList)
        {
            data.AddRange(v.getData());

        }
        SaveToCSV(data, Application.dataPath + "/SimulationData/" + location+"_"+density + ".csv");
        isDataProcessed = true;
    }

    private void SaveToCSV(List<string[]> data, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var row in data)
            {
                writer.WriteLine(string.Join(",", row)); // Write each row as CSV
            }
        }

        Debug.Log("CSV saved at: " + filePath);
    }
}
