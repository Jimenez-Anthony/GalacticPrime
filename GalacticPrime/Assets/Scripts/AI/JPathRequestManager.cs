using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JPathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static JPathRequestManager instance;
    JPathFinder pathfinder;
    bool isProcessing;

    private void Awake() {
        // Singleton Pattern
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        pathfinder = GetComponent<JPathFinder>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, bool flying = false) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, flying);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext() {
        if (!isProcessing && pathRequestQueue.Count > 0) {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessing = true;
            pathfinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.flying);
        }
    }

    public void FinishedProcessing(Vector3[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessing = false;
        TryProcessNext();
    }

    struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public bool flying;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback, bool _flying = false) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            flying = _flying;
        }
    }
}
