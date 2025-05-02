using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlgorithmFactory{
    public static IAlgorithm Create(AlgorithmType algorithm, int quantum){
        switch (algorithm){
            case AlgorithmType.FCFS:
                return new FCFS();
            case AlgorithmType.SJF:
                return new SJF();
            case AlgorithmType.RR:
                return new RR(quantum);
            case AlgorithmType.PriorityBatch:
                return new PriorityBatch();
            case AlgorithmType.PriorityRR:
                return new PriorityRR(quantum);
            default:
                throw new System.ArgumentException("Invalid algorithm type");
        }
    }
}
