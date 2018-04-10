using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJump : MonoBehaviour {
    public GameObject point1;
    public GameObject point2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(point1.transform.position, point2.transform.position);
        Gizmos.DrawLine(point2.transform.position, new Vector3(point2.transform.position.x,point2.transform.position.y,point1.transform.position.z));
        Gizmos.DrawLine(point1.transform.position, new Vector3(point1.transform.position.x, point2.transform.position.y, point1.transform.position.z));
    }
}
