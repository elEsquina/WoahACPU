using UnityEngine;
using System.Collections.Generic;

public class PriorityRR : IAlgorithm
{
    public int Quantum {get; set; } 

    public PriorityRR(int quantum) {
        Debug.Log($"RR constructor called with quantum: {quantum}");
        Quantum = quantum;
    }

    public SelectedProcess SelectNext(List<Process> readyQueue, int currentTime){
        if (readyQueue.Count == 0) {
            return new SelectedProcess { process = null, TTL = 0 }; // No processes to select from
        }

        // Sort the readyQueue based on priority (lower number = higher priority)
        readyQueue.Sort((p1, p2) => p1.priority.CompareTo(p2.priority));

        return new SelectedProcess { 
            process = readyQueue[0],
            TTL = Mathf.Min(this.Quantum, readyQueue[0].remainingTime) 
        };
        
    }
}