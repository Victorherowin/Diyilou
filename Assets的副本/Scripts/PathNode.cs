using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Path
{
    internal class PathNode : MonoBehaviour
    {
        [Serializable]
        public class PathLink
        {
            public PathNode NextNode;
            public float Distance;
            public Vector3 NextNodeDir;

            public Func<EntityBase,Vector3> DirectionFunc;
            public Func<float, Vector3> CurveFunc;
        }


        public bool AllowEnter = true;
        public bool AllowExit = true;
        public bool IsCurve = false;

        public bool IsTerminal = false;

        public PathNode Previous;
        public List<PathLink> ConnectLinks;
        private Dictionary<PathNode, PathLink> m_cacheLink=new Dictionary<PathNode, PathLink>(4);

        private void Awake()
        {
            ConnectLinks.Capacity = 4;
            InitNode();

            IsTerminal = ConnectLinks.Count == 1 ? true : false;
        }

        public PathLink GetLink(PathNode next)
        {
            return m_cacheLink[next];
        }

        /// <summary>
        /// 根据EntityBase的位置获取当前曲线切线方向
        /// </summary>
        /// <param name="e"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private Vector3 GetCurveDirection(EntityBase e,PathNode n)
        {
            var c2n = n.transform.position - transform.position;
            var c2e = e.transform.position - transform.position;
            var c2nl = c2n.magnitude;
            var c2el = c2e.magnitude;

            float x = Vector3.Dot(c2n, c2e) / (c2nl);//cosA*|a|=(a*b)/|b|

            float t = x / c2nl;//t∈[0,1]

            if (t < 0) return GetLineDirection(e, this);
            //if (t > 1) return GetDirection(e, n);

            var a = Curve(transform.position, n.transform.position, t);
            var b = Curve(transform.position, n.transform.position, t + 0.05f);
            var tv = b - a;

            /*if (PointLineDisstance(a,b,e.transform.position)>0.05f)
                tv= b - e.transform.position;*/

            return tv.normalized;
        }

        /// <summary>
        /// 根据EntityBase的位置获取当前直线方向
        /// </summary>
        /// <param name="e"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private Vector3 GetLineDirection(EntityBase e, PathNode n)
        {
            if (PointLineDisstance(transform.position, n.transform.position, e.transform.position) > 0.05f)
                return (n.transform.position - e.transform.position).normalized;
            return (n.transform.position - transform.position).normalized;
        }

        public float GetTFormPosition(EntityBase e,PathNode n)
        {
            var c2n = n.transform.position - transform.position;
            var c2e = e.transform.position - transform.position;
            var c2nl = c2n.magnitude;
            var c2el = c2e.magnitude;

            float x = Vector3.Dot(c2n, c2e) / (c2nl);//cosA*|a|=(a*b)/|b|

            float t = x / c2nl;//t∈[0,1]
            return t;
        }

        /// <summary>
        /// 点到直线的距离
        /// </summary>
        /// <param name="a">直线起点</param>
        /// <param name="b">直线终点</param>
        /// <param name="p">点</param>
        /// <returns></returns>
        private float PointLineDisstance(Vector3 a,Vector3 b,Vector3 p)
        {
            return Vector3.Cross((p - a), (p - b)).magnitude / (b - a).magnitude;
        }

        /// <summary>
        /// 创建Link
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private PathLink CreateLink(PathNode n)
        {
            var dir = (n.transform.position - transform.position);
            PathLink link = new PathLink()
            {
                Distance = dir.magnitude,
                NextNodeDir = dir.normalized,
                NextNode = n,
            };

            if(IsCurve&&n.IsCurve)
            {
                link.CurveFunc=(t) => Curve(transform.position, n.transform.position, t);
                link.DirectionFunc = (e) => GetCurveDirection(e, n);
            }
            else
            {
                link.CurveFunc = (t) => Line(transform.position, n.transform.position, t);
                link.DirectionFunc = (e) => GetLineDirection(e, n);
            }
            return link;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitNode()
        {
            ConnectLinks.Clear();
            var parent = transform.parent;

            if (transform.childCount != 0)
            {
                var t = transform.GetChild(0).GetComponent<PathNode>();
                if (t != null)
                {
                    var link=CreateLink(t);
                    ConnectLinks.Add(link);
                    m_cacheLink[t] = link;
                }
                t.Previous = this;
            }

            if (transform.GetSiblingIndex() == 0)
            {
                var t = parent.GetComponent<PathNode>();
                if (t != null)
                {
                    var link = CreateLink(t);
                    ConnectLinks.Add(link);
                    m_cacheLink[t] = link;
                }
            }
            int index = transform.GetSiblingIndex();

            if (index < parent.childCount - 1)
            {
                var t = parent.GetChild(index + 1).GetComponent<PathNode>();
                if (t != null)
                {
                    var link = CreateLink(t);
                    ConnectLinks.Add(link);
                    m_cacheLink[t] = link;
                }
                t.Previous = this;
            }

            if (index > 0)
            {
                var t = parent.GetChild(index - 1).GetComponent<PathNode>();
                if (t != null)
                {
                    var link = CreateLink(t);
                    ConnectLinks.Add(link);
                    m_cacheLink[t] = link;
                }
            }
        }

        public float Curvature = 0.0f;
        private const float _step = 0.1f;

        /// <summary>
        /// 二次样条插值方程
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        /// <param name="t">t</param>
        /// <returns></returns>
        private Vector3 Curve(Vector3 a, Vector3 b, float t)
        {
            Vector3 tv = b - a;
            Vector3 tp = Vector3.Cross(tv, Vector3.up).normalized * Curvature + (a + tv * 0.5f);

            float tt = t * t;

            return (2*tt-3*t+1)*a+(-4*tt+4*t)*tp+(2*tt-t)*b;
        }

        /// <summary>
        /// 线性插值方程
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 Line(Vector3 a, Vector3 b, float t)
        {
            return a * (1 - t) + b * t;
        }

        private void DrawCurve(PathLink link)
        {
            for (float t = 0.0f; t < 1.0f; t += _step)
            {
                Gizmos.DrawLine(link.CurveFunc(t), link.CurveFunc(t+_step));
            }
        }

        private void OnDrawGizmos()
        {
            InitNode();
            IsTerminal = ConnectLinks.Count == 1 ? true : false;
            foreach (var link in ConnectLinks)
            {
                var c = Gizmos.color;
                if (AllowEnter == true && AllowExit == false)
                    Gizmos.color = Color.yellow;
                else if (AllowEnter == false && AllowExit == true)
                    Gizmos.color = Color.red;
                else if (AllowEnter == false && AllowExit == false)
                    Gizmos.color = Color.grey;
                else
                    Gizmos.color = Color.green;
                if (IsTerminal == true)
                    Gizmos.color = Color.blue;
                Gizmos.DrawCube(transform.position, new Vector3(0.4f, 0.4f, 0.4f));
                Gizmos.color = c;

                DrawCurve(link);
            }
        }
    }
}