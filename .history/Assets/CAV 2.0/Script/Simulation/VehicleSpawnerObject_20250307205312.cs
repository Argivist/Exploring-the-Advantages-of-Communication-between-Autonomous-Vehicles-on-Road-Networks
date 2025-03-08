using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Navigation;

public class VehicleSpawnerObject : MonoBehaviour
{

    [Header("Vehicle Object")]
    public GameObject vehicleObject;


    [Header("SpawnInfo")]
    
    public int id;
    public VehicleType type;
    [Header("Other")]
    public float spawnInterval;//may not be necessary
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
