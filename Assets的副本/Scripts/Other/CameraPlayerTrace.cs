using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerTrace : MonoBehaviour
{
    public float MoveSmoothTime= 0.3f;
    public float RotateSmoothTime = 0.3f;
    public bool opposite = false;

    private Transform m_playerTransform;
    private Vector3 currentVelocity;
    public Vector3 CAMERA_POSITION_OFFSET = new Vector3();

    private Vector3 right = Vector3.zero;
    private Vector3 currentRotateVelocity;

    void Start () 
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate () {
        float offsetX = CAMERA_POSITION_OFFSET.x;
        float offsetZ = CAMERA_POSITION_OFFSET.z;
        float angle=Vector3.Angle(m_playerTransform.forward, Vector3.forward);
        transform.position = Vector3.SmoothDamp(transform.position, m_playerTransform.position + new Vector3(CAMERA_POSITION_OFFSET.x*Mathf.Cos(angle/180*Mathf.PI), CAMERA_POSITION_OFFSET.y, CAMERA_POSITION_OFFSET.x*Mathf.Sin(angle / 180 * Mathf.PI)), ref currentVelocity, MoveSmoothTime);

        int i = 1;
        if (opposite)
            i = -1;

        right = Vector3.SmoothDamp(right, m_playerTransform.right*i, ref currentRotateVelocity, RotateSmoothTime);
        transform.LookAt(transform.position - right);
    }
  
}
