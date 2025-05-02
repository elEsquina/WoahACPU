using UnityEngine;
using System;
using System.Collections.Generic;

public class Measurer : MonoBehaviour
{
    [SerializeField] private Scheduler scheduler;

    void Awake()
    {
        // Allow inspector override, otherwise try to get on same GameObject
        if (scheduler == null)
            scheduler = GetComponent<Scheduler>();

        if (scheduler == null)
        {
            Debug.LogError("Measurer: Scheduler reference not set!");
            enabled = false;
        }
    }

    /// <summary>
    /// General helper to compute an average metric over finished processes.
    /// </summary>
    private bool TryComputeAverage(Func<Process, double> selector, out double average)
    {
        average = 0.0;

        if (scheduler == null || scheduler.finishedProcesses == null)
            return false;

        var processes = scheduler.finishedProcesses;
        int count = 0;
        double sum = 0.0;

        for (int i = 0; i < processes.Count; i++)
        {
            var p = processes[i];
            if (p == null) continue;
            sum += selector(p);
            count++;
        }

        if (count == 0)
            return false;

        average = sum / count;
        return true;
    }

    public double CalculateAverageTurnaroundTime()
    {
        Debug.Log("Measurer: Calculating average turnaround time...");
        if (TryComputeAverage(p => p.GetTurnaroundTime(), out var avg))
        {
            Debug.Log($"Measurer: Average turnaround time = {avg:F2}");
            return avg;
        }

        Debug.LogWarning("Measurer: No finished processes available for turnaround average.");
        return 0.0;
    }

    public double CalculateAverageWaitingTime()
    {
        Debug.Log("Measurer: Calculating average waiting time...");
        if (TryComputeAverage(p => p.GetWaitingTime(), out var avg))
        {
            Debug.Log($"Measurer: Average waiting time = {avg:F2}");
            return avg;
        }

        Debug.LogWarning("Measurer: No finished processes available for waiting average.");
        return 0.0;
    }

    public double CalculateAverageResponseTime()
    {
        Debug.Log("Measurer: Calculating average response time...");
        if (TryComputeAverage(p => p.GetResponseTime(), out var avg))
        {
            Debug.Log($"Measurer: Average response time = {avg:F2}");
            return avg;
        }

        Debug.LogWarning("Measurer: No finished processes available for response average.");
        return 0.0;
    }

    public double CalculateThroughput()
    {
        if (scheduler == null)
        {
            Debug.LogWarning("Measurer: Scheduler reference missing for throughput calculation.");
            return 0.0;
        } 

        int count = scheduler.finishedProcesses.Count;
        int totalTime = scheduler.CurrentTime;

        Debug.Log($"Measurer: Calculating throughput with count={count}, time={totalTime}");

        if (totalTime <= 0)
        {
            Debug.LogWarning($"Measurer: CurrentTime ({totalTime}) <= 0; defaulting time to 1 for throughput.");
            totalTime = 1;
        }

        double throughput = (double)count / totalTime;
        Debug.Log($"Measurer: Throughput = {throughput:F2}");
        return throughput;
    }
}