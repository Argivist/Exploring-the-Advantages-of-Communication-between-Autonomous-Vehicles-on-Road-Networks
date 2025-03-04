using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Description : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject ThisCar;
[Header("Description")]
public int id;


    [Header("Misc")]
    public bool NotMaster=true;
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
