using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class CirclePoint : MonoBehaviour {
	public float radius=5;
	public float angle=0f;
	private float x0, z0;
    public float height = 5f;
    public float centerLength = 5f;
    private Vector3 centerPoint,pos,pos2;
    // Use this for initialization
    void Start () {
        pos = transform.position + transform.forward * radius;
        float hd = angle / 180 * Mathf.PI;
        float x1 = (pos.x - transform.position.x) * Mathf.Cos(hd) + (pos.z - transform.position.z) * Mathf.Sin(hd) + transform.position.x;
        float z1 = -(pos.x - transform.position.x) * Mathf.Sin(hd) + (pos.z - transform.position.z) * Mathf.Cos(hd) + transform.position.z;
        pos2 = new Vector3(x1, transform.position.y, z1);
        Vector3 dir2 = Vector3.Normalize(pos2 - transform.position) * 100;
        Vector3 cirPoint2 = pos2 + dir2;
        Vector3 dir = Vector3.Normalize(pos - transform.position) * 100;
        Vector3 cirPoint = pos + dir;
        Vector3 centerDir = Vector3.Normalize((pos + pos2) / 2 - transform.position);
        centerPoint = transform.position + centerDir * centerLength;
    }
	
	// Update is called once per frame
	void Update () {
        if (isPointInRect(GameObject.FindGameObjectWithTag("Player").transform.position))
        {
            (GameObject.FindGameObjectWithTag("Player")).GetComponent<PlayerMove>().circlePoint = transform.position;
            (GameObject.FindGameObjectWithTag("Player")).GetComponent<PlayerMove>().OnCircle = true;
        }
        else {
            if((GameObject.FindGameObjectWithTag("Player")).GetComponent<PlayerMove>().circlePoint==transform.position)
            (GameObject.FindGameObjectWithTag("Player")).GetComponent<PlayerMove>().OnCircle = false;
        }
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (transform.position, transform.position+transform.forward*radius);
		pos = transform.position + transform.forward * radius;
        float hd = angle / 180 * Mathf.PI;
        float x1 = (pos.x - transform.position.x) * Mathf.Cos(hd) + (pos.z - transform.position.z) * Mathf.Sin(hd) + transform.position.x;
        float z1 = -(pos.x - transform.position.x) * Mathf.Sin(hd) + (pos.z - transform.position.z) * Mathf.Cos(hd) + transform.position.z;
        Gizmos.DrawLine (transform.position, new Vector3(x1,transform.position.y,z1));
        Gizmos.color = Color.red;
        pos2 = new Vector3(x1, transform.position.y, z1);
        Vector3 dir2 = Vector3.Normalize(pos2 - transform.position)*100;
        Vector3 cirPoint2 = pos2 + dir2;
        float x2 = (cirPoint2.x - pos2.x) * Mathf.Cos(-Mathf.PI / 2) + (cirPoint2.z - pos2.z) * Mathf.Sin(-Mathf.PI / 2) + pos2.x;
        float z2 = -(cirPoint2.x - pos2.x) * Mathf.Sin(-Mathf.PI / 2) + (cirPoint2.z - pos2.z) * Mathf.Cos(-Mathf.PI / 2) + pos2.z;
        Gizmos.DrawLine(pos2, new Vector3(x2, pos2.y, z2));

        Vector3 dir= Vector3.Normalize(pos - transform.position) * 100;
        Vector3 cirPoint = pos + dir;
        float x3 = (cirPoint.x - pos.x) * Mathf.Cos(Mathf.PI / 2) + (cirPoint.z - pos.z) * Mathf.Sin(Mathf.PI / 2) + pos.x;
        float z3 = -(cirPoint.x - pos.x) * Mathf.Sin(Mathf.PI / 2) + (cirPoint.z - pos.z) * Mathf.Cos(Mathf.PI / 2) + pos.z;
        Gizmos.DrawLine(pos, new Vector3(x3, pos.y, z3));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + height, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), new Vector3(pos.x, pos.y + height, pos.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), new Vector3(pos2.x, pos2.y + height, pos2.z));
        Gizmos.DrawLine(pos2, new Vector3(pos2.x, pos2.y + height, pos2.z));
        Gizmos.DrawLine(pos, new Vector3(pos.x, pos.y + height, pos.z));

        Vector3 centerDir = Vector3.Normalize((pos + pos2) / 2-transform.position);
        centerPoint = transform.position + centerDir * centerLength;
        Gizmos.DrawLine(transform.position, centerPoint);
        Gizmos.DrawLine(centerPoint, new Vector3(centerPoint.x, centerPoint.y + height, centerPoint.z));
        Gizmos.DrawLine(pos, centerPoint);
        Gizmos.DrawLine(pos2, centerPoint);
        Gizmos.DrawLine(new Vector3(pos2.x, pos2.y + height, pos2.z), new Vector3(centerPoint.x, centerPoint.y + height, centerPoint.z));
        Gizmos.DrawLine(new Vector3(pos.x, pos.y + height, pos.z), new Vector3(centerPoint.x, centerPoint.y + height, centerPoint.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), new Vector3(centerPoint.x, centerPoint.y + height, centerPoint.z));
    }

    bool isPointInRect(Vector3 PlayerPos)
    {
        float x = PlayerPos.x;
        float z = PlayerPos.z;
        float a = (centerPoint.x - pos.x) * (z - pos.z) - (centerPoint.z - pos.z) * (x - pos.x);
        float b = (pos2.x - centerPoint.x) * (z - centerPoint.z) - (pos2.z - centerPoint.z) * (x - centerPoint.x);
        float c = (transform.position.x - pos2.x) * (z - pos2.z) - (transform.position.z - pos2.z) * (x - pos2.x);
        float d = (pos.x - transform.position.x) * (z - transform.position.z) - (pos.z - transform.position.z) * (x - transform.position.x);
        //Debug.Log(a + ":" + b + ":" + c + ":" + d);
        if (((a > 0 && b > 0 && c > 0 && d > 0) || (a < 0 && b < 0 && c < 0 && d < 0))&&(PlayerPos.y>=transform.position.y&& PlayerPos.y <= transform.position.y+height))
        {
            return true;
        }

        //      AB X AP = (b.x - a.x, b.y - a.y) x (p.x - a.x, p.y - a.y) = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);  
        //      BC X BP = (c.x - b.x, c.y - b.y) x (p.x - b.x, p.y - b.y) = (c.x - b.x) * (p.y - b.y) - (c.y - b.y) * (p.x - b.x);  
        return false;
    }
}
