using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class JEnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 50f;
    public float obstacleCheckLength = 1.5f;
    public float playerFollowRange = 30f;
    public float nextWaypointDistance = 0.7f;
    public float pathUpdateCooldown = 0.1f;

    private Vector3[] path;
    private Path fpath;
    private int waypointIndex;
    private Vector3 waypoint;
    int targetIndex;

    private JCharacterController controller;
    private JFlyerController fcontroller;
    private Rigidbody2D rb2d;
    private Collider2D collider;
    private Pathfinding.Seeker seeker;

    // Extra AI Behavior
    private bool flying = false;
    public bool dodgeBullets = false;
    private bool reflectBullets = false;
    private Object bulletLeft;
    private Object bulletRight;

    public LayerMask jumpLayer = 10;

    private int horizontalDir;

    void Start() {
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        if (target == null) {
            target = GameMaster.instance.player.transform;
        }

        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        if (!flying) {
            controller = GetComponent<JCharacterController>();
        }
        else {
            fcontroller = GetComponent<JFlyerController>();
            seeker = gameObject.AddComponent<Pathfinding.Seeker>();
            seeker.drawGizmos = false;
        }
            
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        waypointIndex = 0;
        StartCoroutine(UpdatePath());

        bulletLeft = Resources.Load("BulletToLeft");
        bulletRight = Resources.Load("BulletToRight");
    }

    public void OnPathFound(Vector3[] _path, bool pathSuccessful) {
        if (pathSuccessful) {
            path = _path;

        }
    }

    IEnumerator UpdatePath() {
        while (true) {
            if (Vector3.Distance(transform.position, target.position) < playerFollowRange) {
                yield return new WaitForSeconds(pathUpdateCooldown);
                //if (!flying && Within(rb2d.velocity.y, 0f, 0.01f)) {
                    //Debug.Log("Updating path");
                    JPathRequestManager.RequestPath(transform.GetChild(0).position, target.position, OnPathFound, flying);
                    waypointIndex = 0;
                //}
                if (flying) {
                    //Debug.Log("Updating path");
                    //JPathRequestManager.RequestPath(transform.position, target.position, OnPathFound, flying);
                    //waypointIndex = 0;
                }
            }
            else {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void Update() {
        //print(rb2d.velocity.y);
        if (path == null) {
            return;
        }
        if (waypointIndex < path.Length) {
            waypoint = path[waypointIndex];
            if (path == null) {
                return;
            }
            if (WithinWalking(transform.position, path[waypointIndex], nextWaypointDistance)) {
                waypointIndex++;
                if (waypointIndex >= path.Length) {
                    return;
                }
            }
        }

    }

    void FixedUpdate() {
        if (!flying) {
            NotFlyingMove();
        }
        else {
            FlyingMove();
        }
    }

    void NotFlyingMove() {
        bool jump = false;
        if (dodgeBullets) {
            Collider2D col = Physics2D.OverlapCircle(transform.position, 1f);
            if (col.tag == "Bullet") {
                if (transform.localScale.x > 0 && col.transform.position.x < transform.position.x) {
                    jump = true;
                    waypointIndex++;
                }
                else if (transform.localScale.x < 0 && col.transform.position.x > transform.position.x) {
                    jump = true;
                    waypointIndex++;
                }

            }
        }


        if (path == null) {
            return;
        }
        if (waypointIndex < path.Length) {

            //print("Current y " + transform.position.y + ", target y: " + path[waypointIndex].y);

            //if (!Within(path[waypointIndex].y, transform.position.y, 0.3f)) {
            //    if (path[waypointIndex].y > transform.position.y && !flying) {
            //        print("Waypoint higher");
            //        jump = true;
            //    }
            //}

            horizontalDir = 0;

                if (path[waypointIndex].x < transform.position.x) {
                    //print("Waypoint to the left");
                    horizontalDir = -1;
                }
                else {
                    //print("Waypoint to the right");
                    horizontalDir = 1;
                }
            

            Vector2 dir;
            if (horizontalDir == -1) {
                //print("checking left");
                 dir = Vector2.left;
            }
            else {
                //print("checking right");
                 dir = Vector2.right;
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).position, dir, obstacleCheckLength, jumpLayer);

            if (hit) {
                //print(hit.transform.name + " " + Time.time);
            }

            if (hit && hit.transform.tag == "Ground" && controller.m_Grounded) {
                jump = true;
            }

            if (Within(path[waypointIndex].x, transform.position.x, 0.05f)) {
                controller.Move(0f, false, target.transform);
                //horizontalDir = 0;
            }

            //if (Within(path[waypointIndex].x, transform.position.x, nextWaypointDistance)) {
            //    controller.Move(0f, jump, target.transform);
            //    //horizontalDir = 0;
            //}

            else if (Within(path[waypointIndex].x, transform.position.x, 0.2f)) {
                controller.Move(horizontalDir * Time.fixedDeltaTime * speed / 5f, jump, target.transform);
            }
            else {
                controller.Move(horizontalDir * Time.fixedDeltaTime * speed, jump, target.transform);
            }




           
        }
    }

    void FlyingMove() {

        if (path == null) {
            return;
        }
        if (waypointIndex < path.Length) {
            if (!WithinFlying(target.position, transform.position, 0.1f)) {
                float moveX = 0f;
                float moveY = 0f;
                if (!Within(path[waypointIndex].x, transform.position.x, 0.1f)) {
                    if (path[waypointIndex].x < transform.position.x) {
                        moveX = -1 * Time.fixedDeltaTime * speed;
                    }
                    else {
                        moveX = 1 * Time.fixedDeltaTime * speed;
                    }
                }
                if (!Within(path[waypointIndex].y, transform.position.y, 0.2f)) {
                    if (path[waypointIndex].y < transform.position.y) {
                        moveY = -1 * Time.fixedDeltaTime * speed;
                    }
                    else {
                        moveY = 1 * Time.fixedDeltaTime * speed;
                    }
                }
                fcontroller.Move(moveX, moveY, target.transform);
            }
        }
    }
    
    private bool Within(float a, float b, float margin) {
        return Mathf.Abs(a - b) < margin;
    }
    private bool WithinWalking(Vector2 a, Vector2 b, float margin) {
        return Within(a.x, b.x, margin) && Within(a.y, b.y, margin);
    }
    private bool WithinFlying(Vector2 a, Vector2 b, float margin) {
        return Within(a.x, b.x, margin) && Within(a.y, b.y, margin);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.GetChild(0).position, transform.GetChild(0).position + new Vector3(obstacleCheckLength * horizontalDir, 0f, 0f));
        if (path == null) 
            return;
        
        Gizmos.color = Color.red;
        for (int i = 0; i < path.Length - 1; i++) {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }
}
