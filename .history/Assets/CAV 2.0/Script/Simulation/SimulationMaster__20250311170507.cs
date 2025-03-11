using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationMaster_ : MonoBehaviour
{
    [Header("Simulation Master")]

    public StopWatch sw;

    // Start is called before the first frame update
    void Start()
    {
        sw=gameObject.AddComponent<StopWatch>();
        sw.startTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(sw.getTime()>100){
            sw.stopTimer();
            gameObject.GetComponentInParent<SimGroupAutomate>().EndSimulation();
        }
    }
}
