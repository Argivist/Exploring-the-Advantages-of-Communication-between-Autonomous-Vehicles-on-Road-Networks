using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Description : MonoBehaviour
{
    public GameObject ThisCar
    // Start is called before the first frame update
    void Start()
    {
        
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
