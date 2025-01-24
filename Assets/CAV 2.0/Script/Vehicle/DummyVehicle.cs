using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyVehicle : MonoBehaviour
{
    //Random destroy time
    public int randTime;
    public Timer timer;

    SimulationMaster simMaster;
    // Start is called before the first frame update
    void Start()
    {
        randTime=Random.Range(0,100);
        simMaster = GameObject.Find("SimMaster").GetComponent<SimulationMaster>();
        timer = simMaster.GetComponent<Timer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timer.timer>=randTime){
            simMaster.NumDestroyedVehicles++;
            Destroy(gameObject);
        }
    }
}
