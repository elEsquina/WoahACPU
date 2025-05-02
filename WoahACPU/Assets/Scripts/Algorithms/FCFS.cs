using UnityEngine;
using System.Collections.Generic;

public class FCFS : IAlgorithm{

    public int? Quantum {get; set; } // Not used in FCFS

    public SelectedProcess SelectNext(List<Process> readyQueue, int currentTime){
        if (readyQueue.Count > 0) {
            Process selectedProcess = readyQueue[0]; 
            return new SelectedProcess { process = selectedProcess, TTL = selectedProcess.burstTime };
        }
        return new SelectedProcess { process = null, TTL = 0 }; 
    }

}
