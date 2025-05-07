# app.py
import random
import json
from flask import request, abort, Flask, jsonify, render_template

# In‑memory store of benchmark results
# Each entry is expected to be a dict:
#   { "algorithm": "<name>", "metrics": { "avg_wait":…, "avg_turnaround":…, … } }
benchmarks = []

app = Flask(__name__)


@app.route('/', methods=['GET'])
def get_processes():
    with open('processes.json', 'r') as f:
        try:
            processes = json.load(f)
        except json.JSONDecodeError:
            abort(500, description="Error reading processes.json")
    return jsonify(processes)


@app.route('/generate', methods=['POST'])
def generate_processes():
    """
    Payload has max, min params 
    Generate new processes based on the payload
    Example payload:
    {
        "maxburst": 10,
        "minburst": 1,
        "maxarrival": 20,
        "minarrival": 0,
        "maxpriority": 5,
        "minpriority": 1,
        "count": 5
    }
    """
    data = request.get_json()
    if not data:
        abort(400, description="JSON body required")

    # required keys
    keys = ["minburst","maxburst","minarrival","maxarrival","minpriority","maxpriority","count"]
    missing = [k for k in keys if k not in data]
    if missing:
        abort(400, description=f"Missing fields: {', '.join(missing)}")

    # extract and validate ranges
    minb, maxb = data["minburst"], data["maxburst"]
    mina, maxa = data["minarrival"], data["maxarrival"]
    minp, maxp = data["minpriority"], data["maxpriority"]
    count = data["count"]

    if not all(isinstance(v, int) for v in [minb,maxb,mina,maxa,minp,maxp,count]):
        abort(400, description="All parameters must be integers")
    if minb > maxb or mina > maxa or minp > maxp:
        abort(400, description="Each min must be ≤ its corresponding max")
    if count < 1:
        abort(400, description="count must be at least 1")

    processes = []
    for i in range(1, count+1):
        burst = random.randint(minb, maxb)
        arrival = random.randint(mina, maxa)
        priority = random.randint(minp, maxp)
        processes.append({
            "id": i,
            "arrivalTime": arrival,
            "burstTime": burst,
            "priority": priority,
            "remainingTime": burst,
            "startTime": None,
            "endTime": None
        })

    # Optionally sort by arrivalTime so client sees them in time order:
    processes.sort(key=lambda p: p["arrivalTime"])
    with open('processes.json', 'w') as f:
        json.dump(processes, f)
        
    return jsonify(processes)

@app.route('/benchmark', methods=['POST'])
def post_benchmark():
    """
    POST /benchmark
    ----------------
    Accepts a JSON payload containing performance metrics for a scheduling algorithm,
    and stores it in the in‑memory benchmark list.

    Expected JSON body:
    {
        "algorithm": "<string>",        # Name of the algorithm, e.g. "RR", "FCFS"
        "metrics": {                    # Dictionary of measured metrics
            "avg_wait": <number>,       # Average waiting time
            "avg_turnaround": <number>, # Average turnaround time
            ...                        # Any additional metrics
        }
    }

    Responses:
      201 Created
        {"status": "ok"}
      400 Bad Request
        If the JSON body is missing or does not include both "algorithm" and "metrics".
    """
    data = request.get_json()
    if not data:
        abort(400, description="JSON body required")
    if 'algorithm' not in data or 'metrics' not in data:
        abort(400, description="Must include 'algorithm' and 'metrics'")
    benchmarks.append(data)
    return jsonify({"status": "ok"}), 201


@app.route('/benchmark', methods=['GET'])
def get_benchmark():
    return render_template('benchmark.html', benchmarks=benchmarks)

if __name__ == '__main__':
    app.run(debug=True)
