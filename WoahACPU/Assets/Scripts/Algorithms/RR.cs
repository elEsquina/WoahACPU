using UnityEngine;
using System.Collections.Generic;

public class RR: IAlgorithm
{
    public int Quantum {get; set; } 

    public RR(int quantum) {
        Debug.Log($"RR constructor called with quantum: {quantum}");
        Quantum = quantum;
    }

    public SelectedProcess SelectNext(List<Process> readyQueue, int currentTime){

        if (readyQueue.Count == 0) {
            return new SelectedProcess { process = null, TTL = 0 }; // No processes to select from
        }

        Process selectedProcess = readyQueue[0];

        return new SelectedProcess { 
            process = selectedProcess,
            TTL = Mathf.Min(this.Quantum, selectedProcess.remainingTime) 
        };

    }
}
