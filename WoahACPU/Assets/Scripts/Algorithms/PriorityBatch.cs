using UnityEngine;
using System.Collections.Generic;

public class PriorityBatch : IAlgorithm{

    public int? Quantum {get; set; }  // Not used in FCFS

    public SelectedProcess SelectNext(List<Process> readyQueue, int currentTime){

        // Check if the readyQueue is empty
        if (readyQueue.Count == 0) {
            return new SelectedProcess { process = null, TTL = 0 }; // No process to select
        }

        // Sort the readyQueue based on priority (lower number = higher priority)
        readyQueue.Sort((p1, p2) => p1.priority.CompareTo(p2.priority));

        // Select the process with the highest priority (first in the sorted list)
        Process selectedProcess = readyQueue[0];
        return new SelectedProcess { 
            process = selectedProcess,
            TTL = selectedProcess.burstTime
        };
    }
}