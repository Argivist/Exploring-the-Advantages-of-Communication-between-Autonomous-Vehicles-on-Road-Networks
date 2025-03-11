using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationMaster_ : MonoBehaviour


{
public int limit=1000;
    [Header("Simulation Master")]

    public StopWatch sw;
    public SimGroupAutomate sga;

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
        if(sw.getTime()>limit){
            sw.stopTimer();
            dh.
            // wait for data to be processed
            if(sga.dh.isDataProcessed){
                sga.EndOfSimulation();
            }
            
        }
    }
}
