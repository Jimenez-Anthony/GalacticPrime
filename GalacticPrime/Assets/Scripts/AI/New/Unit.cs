using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


	public Transform target;
	float speed = 0;
	Vector3[] path;
	int targetIndex;

	void Start() {
        //print("fuck me in the ass");
		PathRequestManager.RequestPath(transform.GetChild(0).position,target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
            print("start");
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

    IEnumerator UpdatePath() {
        yield return new WaitForSeconds(0.2f);
        PathRequestManager.RequestPath(transform.GetChild(0).position, target.position, OnPathFound);
    }

    //   IEnumerator FollowPath() {
    //	Vector3 currentWaypoint = path[0];
    //	while (true) {
    //		if (transform.position == currentWaypoint) {
    //			targetIndex ++;
    //			if (targetIndex >= path.Length) {
    //				yield break;
    //			}
    //			currentWaypoint = path[targetIndex];
    //		}

    //		transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
    //		yield return null;

    //	}
    //}

    void Update() {

    }

    public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.red;

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
