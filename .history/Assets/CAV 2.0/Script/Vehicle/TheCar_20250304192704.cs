using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheCar : MonoBehaviour
{

    public GameObject ThisCar;
    public bool NotMaster;
    // Start is called before the first frame update
    void Start()
    {
        // Get parent gameobject        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void endOfLife(){
        if(NotMaster){
            //TODO: Record info and others
        Destroy(ThisCar);
        }
    }
}
