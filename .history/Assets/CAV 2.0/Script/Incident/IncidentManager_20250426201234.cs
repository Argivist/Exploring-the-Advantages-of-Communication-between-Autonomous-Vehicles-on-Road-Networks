using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncidentManager : MonoBehaviour
{
    // SECTION: Variables
    public GameObject incidentPrefab;
    StopWatch stopwatch;

    [Header("Incident Variables")]
    public int incidentDuration = 5;
    public int sleepDuration = 5;
    public int incidentSeverity = 1;

    // SECTION: Start Method
    void Start()
    {
        stopwatch = new StopWatch();        
    }

    // SECTION: Update Method
    void Update()
    {

    }
}
