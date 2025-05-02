using UnityEngine;
using System.Collections.Generic;

public class SJF : IAlgorithm
{
    public int? Quantum {get; set; }  // Not used in FCFS

    public SelectedProcess SelectNext(List<Process> readyQueue, int currentTime){
        if (readyQueue.Count == 0) {
            return new SelectedProcess { process = null, TTL = 0 }; // No process to select
        }

        // Sort the readyQueue based on burst time (shortest first)
        readyQueue.Sort((p1, p2) => p1.burstTime.CompareTo(p2.burstTime));

        return new SelectedProcess { 
            process = readyQueue[0],
            TTL = readyQueue[0].burstTime 
        };
    }
}
