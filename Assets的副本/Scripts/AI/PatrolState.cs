using Entity.Monster;
using Game.Path;
using Other;
using System;
using UnityEngine;

internal class PatrolState : FSMState
{
    private Transform playerTransform;
    private Animator m_animator;
    private CharacterController m_controller;
    public PathNode m_start;
    public PathNode m_end;
    private MonsterBase m_entity;
    private PathNode[] m_path;
    private PathNode[] m_backStartPath;

    public float Speed;

    public PatrolState(FSMSystem fsm, MonsterBase obj, PathNode path1, PathNode path2) : base(fsm)
    {
        m_entity = obj;
        m_animator = obj.GetComponent<Animator>();
        m_controller = obj.GetComponentInParent<CharacterController>();
        stateID = StateID.Patrol;
        playerTransform = GameObject.Find("Player").transform;

        m_start = path1;
        m_end = path2;
        //找到两个巡逻点
        m_path = PathManager.Instance.FindPath(m_start, m_end);

        Speed = obj.MoveSpeed;
    }

    private enum Status
    {
        Front, Rotate, BackToPath, WarpToPath
    }

    private Status _status = Status.Front;

    public override void DoBeforeEntering()
    {
        Speed = Mathf.Abs(Speed);
        m_backStartPath = PathManager.Instance.FindPath(m_entity.LastPassNode, m_start);
        if (m_backStartPath.Length == 0)
        {
            _status = Status.WarpToPath;
            return;
        }

        bool flag = false;

        //是否需要返回巡逻路径
        if (Array.IndexOf(m_path, m_entity.LastPassNode) >= 0)//可能在巡逻路径中
        {
            foreach (var n in m_path)
                foreach (var link in n.ConnectLinks)
                    if (link.NextNode == m_entity.SelectLink.NextNode && Array.IndexOf(m_path, link.NextNode) < 0)
                    {
                        _status = Status.BackToPath;
                        flag = true;
                    }
        }
        else
        {
            _status = Status.BackToPath;
            flag = true;
        }

        if (flag)
        {
            var dir = (m_path[0].transform.position-m_entity.ParentTransform.position).normalized;
            if (Vector3.Dot(m_entity.ParentTransform.forward, dir) < 0.0f)
                Speed = -Mathf.Abs(Speed);
            else
                Speed = Mathf.Abs(Speed);
        }
        else
            _status = Status.Front;
    }

    //巡逻
    public override void Act(MonsterBase npc, Animator ani)
    {
        //单节点，不巡逻
        if (_status == Status.Front && m_start != m_end)
        {
            float dist = Vector3.Distance(m_end.transform.position, npc.ParentTransform.position);

            if (npc.LastPassNode != m_end && dist > 0.8f)
            {
                //var t = npc.transform.forward * 5.0f * Time.deltaTime;
                m_entity.Move(Speed);
                ani.SetFloat("speed", 1.0f);
            }
            else
            {
                if (npc.LastPassNode == m_end && dist <= 0.8f)
                    _status = Status.Rotate;
            }
        }
        else if (_status == Status.Rotate)
        {
            ani.SetFloat("speed", 0.0f);
            var tmp = m_end;
            m_end = m_start;
            m_start = tmp;
            Speed = -Speed;
            _status = Status.Front;
        }
        else if (_status == Status.BackToPath)
        {
            ani.SetFloat("speed", 1.0f);
            npc.Move(Speed);
            if (m_start == m_end)
            {
                if (Vector3.Distance(m_entity.ParentTransform.position, m_start.transform.position) < 0.5f)
                {
                    _status = Status.Front;
                    ani.SetFloat("speed", 0.0f);
                    m_entity.Move(0);
                }
            }
            else if (Vector3.Distance(m_entity.ParentTransform.position, m_path[0].GetLink(m_path[1]).CurveFunc(0.5f)) < 0.5f)
            {
                _status = Status.Front;
                ani.SetFloat("speed", 0.0f);
                m_entity.Move(0);
                if (m_entity.LastPassNode == m_end)
                {
                    var tmp = m_end;
                    m_end = m_start;
                    m_start = tmp;
                }
            }
        }
        else if (_status == Status.WarpToPath)
        {
            var p = Camera.main.WorldToScreenPoint(npc.ParentTransform.position);

            if ((p.x < 0 || p.x > Screen.width) || (p.y < 0 || p.y > Screen.height))
            {
                npc.ParentTransform.position = m_start.transform.position;
                _status = Status.Front;
            }
        }
    }

    public override void Reason(MonsterBase npc, Animator ani)
    {
        if (Vector3.Distance(playerTransform.position, npc.transform.position) < npc.SeePlayerDistance)
        {
            var o = npc.transform.position + new Vector3(0, 0.8f, 0);
            var t = playerTransform.position + new Vector3(0, 0.5f, 0);
            Ray lookRay = new Ray(o, (t - o).normalized);
            RaycastHit hit;
            if (Physics.Raycast(lookRay, out hit, 10, Layers.Player | Layers.Building))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.DrawRay(npc.transform.position, npc.transform.forward, Color.red, 20f);
                    fsm.PerformTransition(Transition.SeePlayer);
                    ani.SetFloat("speed", 0.0f);
                    m_entity.Move(0);
                }
            }
        }
    }
}