# WoahACPU

**WoahACPU** is a process scheduling simulator developed with love & care for the first assignment of the Operating Systems course.
A detailed Report is available in `Report.pdf`

## Project Info

* **Unity Version:** `2021.3.22f1 (LTS)`
* **Platform:** Cross-platform (Android, Windows)
* **Project Type:** A simulator that visualizes how processes are selected by different scheduling algorithms.

## Unity Distribution Options

This project is available in two formats:

1. **ZIP Archive** – A complete Unity project with all necessary folders (`Assets`, `Packages`, `ProjectSettings`, etc.).
2. **Folder-Only Version** – Contains only the core `Assets/Scripts` and essential files for review or integration.

## Python Backend

This project also includes a Python-based backend server that benchmarks and serves process information, either randomly generated or loaded from file input `Server/processes.json` .

### 0. Python Backend Setup

1. Ensure Python is installed.
2. Run the following commands:

```bash
cd Server
pip install -r requirements.txt
python3 ProcessServer.py
```

* The backend must be hosted on the **same machine** where the Unity simulator is run.
* Make sure **port 5000** is available and not used by other applications.

## Unity Setup Instructions

### 1. Install Unity

Install [Unity Hub](https://unity.com/download), and ensure **Unity version 2021.3.22f1 (LTS)** is installed.

### 2. Open the Project

1. Extract the ZIP archive.
2. Open **Unity Hub**, click **"Open"**, and select the extracted project folder.

### 3. Run the Project

* Click the **Play** button in the Unity Editor to launch the simulation.

### 4. Build the Project

1. Go to **File > Build Settings**.
2. Select your target platform (e.g., Windows, Android).
3. Click **Build** and choose an output folder.

## Notes

* All C# scripts are located in `Assets/Scripts`.
* If any packages are missing, open **Window > Package Manager** to install them.
