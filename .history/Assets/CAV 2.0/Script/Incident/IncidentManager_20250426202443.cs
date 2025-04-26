using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncidentManager : MonoBehaviour
{
    // SECTION: Variables
    public GameObject incidentPrefab;
    public StopWatch stopwatch;

    [Header("Incident Variables")]
    public int incidentDuration = 5;
    public int sleepDuration = 5;
    public int incidentSeverity = 1;

    [Header("Timer")]
    public float time;

    // SECTION: Start Method
    void Start()
    {
        stopwatch = new StopWatch();
        incidentPrefab.SetActive(false);
        stopwatch.startTimer();

    }

    // SECTION: Update Method
    void Update()
    {
        // Update the stopwatch
        time = stopwatch.getTime();


        // If the incident is not active, check if the stopwatch has reached the sleep duration
        if (stopwatch.getTime() >= sleepDuration && incidentPrefab.activeSelf == false)
        {
            stopwatch.stopTimer();
            stopwatch.resetTimer();
            incidentPrefab.SetActive(true);
            stopwatch.startTimer();
        }

        // If the incident is active, check if the stopwatch has reached the incident duration
        if (stopwatch.getTime() >= incidentDuration && incidentPrefab.activeSelf == true)
        {
            stopwatch.stopTimer();
            stopwatch.resetTimer();
            incidentPrefab.SetActive(false);
            stopwatch.startTimer();
        }


    }
}
