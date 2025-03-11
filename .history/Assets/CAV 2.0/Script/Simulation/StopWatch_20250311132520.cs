using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWatch : MonoBehaviour
{
    [Header("StopWatch")]
    public int time;

    bool start;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(start){
            time+=1;
        }else{
            time=0;
        }
    }

    void startTimer(){
        start=true;
    }
    void stopTimer(){
        start=false;
    }

    int getTime(){
        return time;
    }
}
