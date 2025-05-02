using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Process {
    public int id;
    public int arrivalTime;
    public int burstTime;
    public int priority;

    public int remainingTime;
    public int startTime = -1;
    public int endTime = -1;


    public void Reset() {
        remainingTime = burstTime;
        startTime = endTime = -1;
    }
    public Process(int id, int arrivalTime, int burstTime, int priority) {
        this.id = id;
        this.arrivalTime = arrivalTime;
        this.burstTime = burstTime;
        this.priority = priority;
        Reset();
    }

    public int GetTurnaroundTime() => endTime - arrivalTime;
    public int GetWaitingTime() => GetTurnaroundTime() - burstTime;
    public int GetResponseTime() => startTime - arrivalTime;
    public bool IsFinished() => remainingTime <= 0;

    public void Run(int currentTime) {
        if (startTime < 0) startTime = currentTime;
        remainingTime = Mathf.Max(0, remainingTime - 1);
        if (IsFinished()) endTime = currentTime;

    }
}
