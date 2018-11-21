using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDemo : MonoBehaviour
{

    public GameObject start;
    public GameObject end;
    public GameObject node;
    // Use this for initialization
    void Start()
    {
        Path p = ABPath.Construct(start.transform.position, end.transform.position, OnPathComplete);
        AstarPath.StartPath(p);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPathComplete(Path p)
    {
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            Debug.LogError(string.Format("x:{0},y:{1},z:{2}", p.vectorPath[i].x, p.vectorPath[i].y, p.vectorPath[i].z));
            GameObject nodeTwo = GameObject.Instantiate(node);
            nodeTwo.transform.position = p.vectorPath[i];
        }
    }
}
