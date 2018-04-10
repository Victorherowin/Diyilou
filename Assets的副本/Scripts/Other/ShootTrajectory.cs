using Entity;
using Other;
using System.Collections.Generic;
using UnityEngine;

namespace Shoot.Trajectory
{
    [RequireComponent(typeof(LineRenderer))]
    internal class ShootTrajectory : MonoBehaviour
    {
        public float Length = 6.0f;
        public float ReflectCount = 5;
        public float StepTime = 0.33f;

        public static ShootTrajectory Instance;

        private LineRenderer m_lineRenderer;
        private Vector3 m_initialVelocity;
        private Vector3 m_downVelocity;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                throw new UnityException("ShootTrajectory.Instance!=null");

            m_lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetInitialVelocity(Vector3 initialVelocity)
        {
            m_initialVelocity = initialVelocity;
            m_downVelocity = Physics.gravity * StepTime;
        }

        private List<Vector3> arr = new List<Vector3>();

        public void ShowTrajectory(Vector3 o)
        {
            arr.Clear();
            arr.Add(o);

            Vector3 old_x = o, new_x = Vector3.zero;
            Vector3 v = m_initialVelocity;
            float length = 0.0f;
            int reflect = 0;
            while (length < Length && reflect < 5)
            {
                new_x = old_x + v * StepTime;

                float delta_length = (new_x - old_x).magnitude;

                RaycastHit hitInfo;
                bool collsion = Physics.Raycast(old_x, v.normalized, out hitInfo, delta_length,Layers.Default);

                if (collsion)
                {
                    v = Vector3.Reflect(v, hitInfo.normal);
                    //t = 0;
                    reflect++;
                    old_x = hitInfo.point;
                }
                else
                {
                    old_x = new_x;
                }
                v = v + m_downVelocity;
                length += delta_length;

                arr.Add(old_x);
            }
            m_lineRenderer.positionCount = arr.Count;
            m_lineRenderer.SetPositions(arr.ToArray());
        }

        public static void DrawTrajectory(EntityBase entity,float v)
        {
            Vector3 pos = entity.transform.Find("ShotLine").position;
            Vector3 dir = entity.transform.Find("ShotLine/Sphere").position - pos;
            dir.Normalize();

            if(Instance!=null)
            {
                Instance.SetInitialVelocity(dir * v);
                Instance.ShowTrajectory(pos);
            }
        }
    }
}