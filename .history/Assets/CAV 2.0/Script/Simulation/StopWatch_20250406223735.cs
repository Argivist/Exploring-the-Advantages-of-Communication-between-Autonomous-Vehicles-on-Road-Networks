using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StopWatch : MonoBehaviour
{
    [Header("StopWatch")]
    public float time;

    bool start;
    [ContextMenu("Start")]
    private void StartTimer(){
        startTimer();
    }
    [ContextMenu("Stop")]
    private void StopTimer(){
        stopTimer();
    }
    [ContextMenu("GetTime")]
    private void getTimer(){
        Debug.Log("Time: "+getTime());
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(start){
            time+=Time.deltaTime;
        }else{
            time=0;
        }
    }

    public void startTimer(){
        start=true;
    }
    public void stopTimer(){
        start=false;
    }

    public int getTime(){
        return time;
    }

    public void resetTimer(){
        start=false;
        time=0;
    }
}
