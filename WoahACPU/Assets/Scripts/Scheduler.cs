using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Scheduler : MonoBehaviour {
    public AlgorithmType algorithm;
    public int quantum = 2;
    public float tickInterval = 0.5f;
    public Process CurrentProcess;
    
    public int Timer = 0;
    public List<Process> allProcesses;
    public List<Process> readyProcesses;
    public List<Process> finishedProcesses;

    public int CurrentTime { get; private set; }

    public event Action TickEvent;

    private IAlgorithm scheduler;

    public void Simulate() {
        readyProcesses = new List<Process>();
        finishedProcesses = new List<Process>();
        scheduler = AlgorithmFactory.Create(algorithm, quantum);
        CurrentTime = -1;
        StartCoroutine(FetchProcesses("http://localhost:5000/"));
    }

    IEnumerator FetchProcesses(string url) {
        using var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success) {
            Debug.LogError(req.error);
            yield break;
        }

        // parse JSON...
        string wrapped = $"{{\"array\":{req.downloadHandler.text}}}";
        var wrapper = JsonUtility.FromJson<JsonArrayWrapper<Process>>(wrapped);
        allProcesses = wrapper.array.ToList();
        allProcesses.ForEach(p => p.Reset());

        InvokeRepeating(nameof(Tick), 0f, tickInterval);
    }


    void CheckForArrivals(){
        var newArrivals = allProcesses.Where(p => p.arrivalTime == CurrentTime).ToList();
        foreach (var process in newArrivals) {
            readyProcesses.Add(process);
            process.Reset();
        }       
    }


    void TimerInterrupt() {
        // Check if the current process is finished
        if (CurrentProcess != null && !CurrentProcess.IsFinished()) {
            readyProcesses.Add(CurrentProcess);
        }
        else if (CurrentProcess != null && CurrentProcess.id != 0) {
            finishedProcesses.Add(CurrentProcess);
            CurrentProcess = null;
        }

        // Select the next process to run
        SelectedProcess info = scheduler.SelectNext(readyProcesses, CurrentTime);
    
        if (info.process != null) {
            readyProcesses.Remove(info.process);
            CurrentProcess = info.process;
            Timer = info.TTL;
        } else {
            CurrentProcess = null;
        }
    }


    void Tick() {
        // if every process has finished, stop the repeating invoke
        if (finishedProcesses.Count == allProcesses.Count) {
            CancelInvoke(nameof(Tick));
            Debug.Log("All processes complete. Stopping scheduler.");
            return;
        }

        // Decrement the timer
        Timer = Mathf.Max(0, Timer - 1);
        CurrentTime++;
        if (CurrentProcess != null) {
            CurrentProcess.Run(CurrentTime);
        } 

        // Check for new arrivals
        CheckForArrivals();

        // Check if the current process is finished or the timer has expired
        if (CurrentProcess == null || CurrentProcess.IsFinished() || Timer == 0)
            TimerInterrupt();

        TickEvent?.Invoke();
    }
}
