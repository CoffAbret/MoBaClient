using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAI : MonoBehaviour {
    private Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    public Path path;
    public float speed = 200;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;

	void Start () {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
	}

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                return;
            if (!hit.transform)
                return;
            targetPosition = hit.point;
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
	}

    private void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
            return;
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove(dir);
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
