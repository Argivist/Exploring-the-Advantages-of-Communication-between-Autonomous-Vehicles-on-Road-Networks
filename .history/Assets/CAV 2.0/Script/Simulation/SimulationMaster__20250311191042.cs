using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;

public class SimulationMaster_ : MonoBehaviour


{
public int limit=1000;
    [Header("Simulation Master")]

    public StopWatch sw;
    public SimGroupAutomate sga;

    public SimulationConfigurer sc;

    [Header("Data Handler")]
    public DataHandler dh;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<StopWatch>();
        sw = gameObject.GetComponent<StopWatch>();
        sw.startTimer();
    }

    // Update is called once per frame
    void Update()
    {
        dh.recordTime(Random.Range(0,4),sw.getTime(), Random.Range(0,100));
        if(sw.getTime()>limit){
            sw.stopTimer();
            dh.ProcessData();
            // wait for data to be processed
            if(sga.dh.isDataProcessed){
                sga.EndOfSimulation();
            }
            
        }
    }
}
