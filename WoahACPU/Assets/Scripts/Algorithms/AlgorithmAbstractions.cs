using UnityEngine;
using System.Collections.Generic;

public enum AlgorithmType{
    FCFS,          // First‑Come, First‑Served (batch)
    SJF,           // Shortest Job First (batch)
    RR,            // Round Robin (interactive)
    PriorityBatch, // Priority Scheduling (batch)
    PriorityRR     // Priority + RR (interactive)
}

public struct SelectedProcess {
    public Process process;
    public int TTL;
}

public interface IAlgorithm {
    SelectedProcess SelectNext(List<Process> readyQueue, int currentTime);
}
