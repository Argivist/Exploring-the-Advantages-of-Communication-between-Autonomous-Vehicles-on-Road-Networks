using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StopWatch : MonoBehaviour
{
    [Header("StopWatch")]
    public int time;

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

    public override void OnInspectorGUI(){

    }
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
