using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [ReadOnly]
    public int timer;

    private bool start, pause, stop, reset,running=false;//TODO - Make private and create public methods to access them
    // Start is called before the first frame update
    void Start()
    {
        timer=0;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            running = true;
        }
        if (pause)
        {
            running = false;
        }
        if (stop)
        {
            start = false;
            running = false;
            
        }
        if (reset)
        {
            timer = 0;
            reset = false;
            start = false;
            pause = false;
            stop = false;
            running = false;
        }
        if (running)
        {
            timer++;
        }
    }

    //TODO - Create public methods to access the private variables
    public void StartTimer()
    {
        timer=0;
        start = true;
    }
    public void PauseTimer()
    {
        pause = true;
    }
    public void ContinueTimer()
    {
        pause = false;
    }
    public void StopTimer()
    {
        stop = true;
    }
    public void ResetTimer()
    {
        reset = true;
    }
    public int GetTimer()
    {
        return timer;
    }
    public bool IsRunning()
    {
        return running;
    }
    public int getStatus()
    {
        if (start)
        {
            return 1;
        }
        if (pause)
        {
            return 2;
        }
        if (stop)
        {
            return 3;
        }
        if (reset)
        {
            return 4;
        }
        return 0;
    }
}
