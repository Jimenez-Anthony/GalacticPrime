using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class JEnemyAIFlying : MonoBehaviour {

    public Transform target; // Target of the enemy
    public float playerFollowRange = 30f;

    private float updateRate = 2f; // Rate of update for the AI

    private Seeker seeker; // Components
    private Rigidbody2D rb;

    private Path path; // Calculated path

    public float speed = 300f; // AI speed
    private ForceMode2D fMode;

    [HideInInspector]
    public bool pathEnded = false;

    // Max distance from enemy to waypoint for it to move on
    public float nextWaypointDistance = 0.3f;

    // Curent waypoint enemy is moving towards
    private int currentWaypoint = 0;

    // Target searching
    private bool searchingForTarget = false;
    private string targetTag = "Player";
    private float searchRate = 2f;

    public Vector3 dir;

    void Start() {
        dir = Vector3.zero;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null) {
            if (!searchingForTarget) {
                searchingForTarget = true;
                StartCoroutine(TargetSearch());
            }
            return;
        }

        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // Start a new path to the target position, run OnPathComplete once completed
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator TargetSearch() {
        GameObject result = GameObject.FindGameObjectWithTag(targetTag);
        if (result == null || Vector3.Distance(result.transform.position, transform.position) > playerFollowRange) {
            yield return new WaitForSeconds(1f / searchRate);
            StartCoroutine(TargetSearch());
        }
        else {
            searchingForTarget = false;
            target = result.transform;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }

    IEnumerator UpdatePath() {

        if (target == null) {
            if (!searchingForTarget) {
                searchingForTarget = true;
                StartCoroutine(TargetSearch());
            }
            yield return false;
        }
        else {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
        yield return new WaitForSeconds(1f / updateRate);

        StartCoroutine(UpdatePath());

    }

    public void OnPathComplete(Path p) {
        if (p.error) {
            Debug.LogError("Error: Pathing for the AI: " + gameObject.name + " could not be completed.");
        }
        else {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate() {
        if (target == null) {
            if (!searchingForTarget) {
                searchingForTarget = true;
                StartCoroutine(TargetSearch());
            }
            return;
        }

        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            if (pathEnded) {
                return;
            }

            pathEnded = true;
            return;
        }
        pathEnded = false;

        // Direction to the next waypoint
        dir = Vector3.Normalize(path.vectorPath[currentWaypoint] - transform.position);
        //dir *= speed;

        // Move the AI
        rb.AddForce(dir * speed, fMode);

        if (Vector3.Distance(new Vector3(transform.position.x, transform.position.y), path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint += 1;
            return;
        }

    }

    private bool withIn(Vector3 a, Vector3 compare, float margin) {
        return Mathf.Abs(a.y - compare.y) < 0.1f && Mathf.Abs(a.x - compare.x) < margin;
    }

}
