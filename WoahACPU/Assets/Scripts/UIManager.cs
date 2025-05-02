using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UIDocument mainUIDocument;
    [SerializeField] private Scheduler scheduler;

    // UI Elements
    private VisualElement root;
    private DropdownField algorithmDropdown;
    private Button startButton;
    private ScrollView readyQueue;
    private VisualElement cpuDisplay;
    private ScrollView finishedQueue;
    private Label timeLabel;
    private Label avgTurnaroundLabel;
    private Label avgWaitingLabel;
    private Label avgResponseLabel;
    private Label throughputLabel;

    private bool isInitialized = false;

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (mainUIDocument == null)
        {
            Debug.LogError("UIManager: UIDocument not set");
            return;
        }

        root = mainUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIManager: Failed to get root");
            return;
        }

        algorithmDropdown = root.Q<DropdownField>("algorithm-dropdown");
        startButton      = root.Q<Button>("start-button");
        readyQueue       = root.Q<ScrollView>("ready-scroll");
        cpuDisplay       = root.Q<VisualElement>("cpu-display");
        finishedQueue    = root.Q<ScrollView>("finished-scroll");
        timeLabel        = root.Q<Label>("time-label");
        avgTurnaroundLabel = root.Q<Label>("turnaround-label");
        avgWaitingLabel  = root.Q<Label>("waiting-label");
        avgResponseLabel = root.Q<Label>("response-label");
        throughputLabel  = root.Q<Label>("throughput-label");

        Debug.Log($"UIManager init: startButton={startButton!=null}, readyQueue={readyQueue!=null}, cpuDisplay={cpuDisplay!=null}, finishedQueue={finishedQueue!=null}");

        if (startButton == null || readyQueue == null || cpuDisplay == null || finishedQueue == null)
        {
            Debug.LogError("UIManager: Missing critical UI elements");
            return;
        }

        startButton.clicked += StartSimulation;
        isInitialized = true;
    }

    private void OnDisable()
    {
        if (startButton != null)
            startButton.clicked -= StartSimulation;
        if (scheduler != null)
            scheduler.TickEvent -= UpdateUI;
    }

    private void StartSimulation()
    {
        if (!isInitialized)
        {
            Debug.LogError("UIManager: Not initialized");
            return;
        }
        if (scheduler == null)
        {
            Debug.LogError("UIManager: Scheduler not set");
            return;
        }

        readyQueue.Clear();
        cpuDisplay.Clear();
        finishedQueue.Clear();

        if (algorithmDropdown != null && Enum.TryParse(algorithmDropdown.value, out AlgorithmType alg))
            scheduler.algorithm = alg;

        scheduler.TickEvent -= UpdateUI;
        scheduler.TickEvent += UpdateUI;
        scheduler.Simulate();
    }

    private void UpdateUI()
    {
        if (!isInitialized || scheduler == null)
        {
            Debug.LogWarning("UIManager: UI not ready");
            return;
        }

        timeLabel.text = $"Time: {scheduler.CurrentTime}";
        UpdateQueue(readyQueue, scheduler.readyProcesses, "ready-process");
        UpdateQueue(finishedQueue, scheduler.finishedProcesses, "finished-process");
        UpdateCPU();
        UpdateStatistics();
    }

    private void UpdateQueue(ScrollView container, List<Process> list, string className)
    {
        container.Clear();
        if (list == null) return;
        foreach (var p in list)
        {
            if (p == null) continue;
            var e = new VisualElement();
            e.AddToClassList("process");
            e.AddToClassList(className);
            e.Add(new Label($"ID: {p.id}"));
            e.Add(new Label($"BT: {p.burstTime}"));
            e.Add(new Label($"RT: {p.remainingTime}"));
            container.Add(e);
        }
        container.scrollOffset = new Vector2(0, container.contentContainer.layout.height);
    }

    private void UpdateCPU()
    {
        cpuDisplay.Clear();
        var p = scheduler.CurrentProcess;
        if (p == null) return;
        var e = new VisualElement();
        e.AddToClassList("process"); e.AddToClassList("cpu-process");
        e.Add(new Label($"ID: {p.id}"));
        e.Add(new Label($"BT: {p.burstTime}"));
        e.Add(new Label($"RT: {p.remainingTime}"));
        cpuDisplay.Add(e);
    }

    // Compute and update stats locally
    private void UpdateStatistics()
    {
        var finished = scheduler.finishedProcesses;
        int count = finished?.Count ?? 0;
        if (count == 0) return;

        double turnaround = ComputeAverage(finished, p => p.GetTurnaroundTime());
        double waiting    = ComputeAverage(finished, p => p.GetWaitingTime());
        double response   = ComputeAverage(finished, p => p.GetResponseTime());
        double throughput = (double)count / Math.Max(scheduler.CurrentTime, 1);

        SetLabel(avgTurnaroundLabel, "Turnaround", turnaround);
        SetLabel(avgWaitingLabel,    "Waiting",    waiting);
        SetLabel(avgResponseLabel,   "Response",   response);
        SetLabel(throughputLabel,    "Throughput", throughput);

        Debug.Log($"Stats: TA={turnaround:F2}, W={waiting:F2}, R={response:F2}, TP={throughput:F2}");
    }

    private double ComputeAverage(List<Process> list, Func<Process, double> selector)
    {
        double sum = 0; int c = 0;
        foreach (var p in list)
            if (p != null) { sum += selector(p); c++; }
        return c > 0 ? sum / c : 0.0;
    }

    private void SetLabel(Label lbl, string prefix, double val)
    {
        if (lbl != null)
            lbl.text = $"{prefix}: {val:F2}";
    }
}