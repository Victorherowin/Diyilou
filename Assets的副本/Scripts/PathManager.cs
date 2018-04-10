using Entity;
using Other;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Path
{
    internal class PathManager : MonoBehaviour
    {
        public PathNode EnterNode;
        public static PathManager Instance;

        private PathNode[] _nodes;

        private void Awake()
        {
            if (Instance != null)
                throw new UnityException("PathManager.Instance!=null");
            Instance = this;
            _nodes = GetComponentsInChildren<PathNode>();
        }

        /// <summary>
        /// 寻找距离Entity最近的节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PathNode FindNearNode(EntityBase entity)
        {
            //LinkedListNode<PathNode>


            float d = float.MaxValue;
            PathNode node = null;
            foreach (var n in _nodes)
            {
                var dir = n.transform.position - entity.transform.position;
                float t = dir.magnitude;
                if (t < d)
                {
                    d = t;
                    node = n;
                }
            }
            return node;
        }

        private static Vector3 _offset = new Vector3(0, 1.6f, 0);

        /// <summary>
        /// 检测所有Entity通过的Node
        /// </summary>
        private void CheckNodePass()
        {
            foreach (var e in EntityBase.AllEntitys)
            {
                RaycastHit hit;

                if (Physics.Raycast(e.transform.position + _offset, Vector3.down, out hit, 100.0f, Layers.Path | Layers.Building))
                {
                    var o = hit.collider.GetComponent<PathNode>();
                    if (o != null)
                    {
                        e.LastPassNode = o;
                        e.OnPassNode(o);

                        Debug.DrawLine(e.transform.position, o.transform.position, Color.red);
                    }
                }
            }
        }
            
        private void Update()
        {
            CheckNodePass();
        }

        private class VIComparer : IComparer<V>
        {
            public int Compare(V x, V y)
            {
                int t = x.MinCost.CompareTo(y.MinCost);
                return t == 0 ? 1 : t;
            }
        }

        private class V
        {
            public float MinCost;
            public PathNode Slef;
            public V Previous;
        }

        /// <summary>
        /// 寻找路径
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        /// <returns>一条可行路径，如果不存在Count==0</returns>
        public PathNode[] FindPath(PathNode a, PathNode b)
        {
            List<V> node_set = new List<V>(_nodes.Length);

            Dictionary<PathNode, V> mapping = new Dictionary<PathNode, V>(_nodes.Length);
            foreach (var n in _nodes)
            {
                V v = new V();
                v.MinCost = n == a ? 0.0f : float.MaxValue;
                v.Slef = n;
                v.Previous = null;

                mapping[n] = v;
                node_set.Add(v);
            }

            while (node_set.Count != 0)
            {
                float min = float.MaxValue;
                V current = node_set[0];
                int index = 0;
                for (int i = 0; i < node_set.Count; i++)
                {
                    if (node_set[i] == null) continue;
                    if (node_set[i].MinCost <= min)
                    {
                        min = node_set[i].MinCost;
                        current = node_set[i];
                        index = i;
                    }
                }

                if (current.Slef == b) break;
                node_set[index] = null;

                foreach (var next in current.Slef.ConnectLinks)
                {
                    if (next.NextNode == current.Slef) continue;
                    if (current.Slef.AllowExit == false && next.NextNode.AllowEnter == false) continue;

                    V nextv = mapping[next.NextNode];
                    float edged = next.Distance;
                    if (edged + current.MinCost < nextv.MinCost)
                    {
                        nextv.MinCost = edged + current.MinCost;
                        nextv.Previous = current;
                    }
                }
            }
            List<PathNode> path = new List<PathNode>(_nodes.Length);
            V end = mapping[b];
            V s = end;

            if (a != b && s.Previous == null)
                return path.ToArray();

            do
            {
                path.Add(s.Slef);
                s = s.Previous;
            } while (s != null && end != s);

            path.Reverse();
            return path.ToArray();
        }
    }
}