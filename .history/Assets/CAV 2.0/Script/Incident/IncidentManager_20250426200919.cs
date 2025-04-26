using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncidentManager : MonoBehaviour
{
    // SECTION: Variables
    public GameObject incidentPrefab;

    // SECTION: Start Method
    void Start()
    {
        // Initialize the incident manager
        InitializeIncidents();
    }
}
