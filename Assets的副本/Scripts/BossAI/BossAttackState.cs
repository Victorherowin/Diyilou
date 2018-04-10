using Entity;
using Entity.Monster;
using Other;
using Skill.Context;
using UnityEngine;

internal class BossAttackState : FSMState
{
    private Status _status = Status.Attack;

    private enum Status
    {
        Idle,
        Attack,
        skill_01,
        skill_02,
    }

    private Vector3 tall = new Vector3(0, 2, 0);
    private Transform playerTransform;
    private Animator m_animator;
    private CharacterController m_controller;
    private MonsterBase m_entity;


    private SkillContext[] m_att;
    private SkillContext[] m_skill;

    public BossAttackState(FSMSystem fsm, MonsterBase obj, SkillContext[] att, SkillContext[] skill) : base(fsm)
    {
        m_animator = obj.GetComponent<Animator>();
        m_controller = obj.GetComponent<CharacterController>();
        stateID = StateID.Attack;
        playerTransform = GameObject.Find("Player").transform;
        m_att = att;
        m_skill = skill;
        m_entity = obj;

        obj.OnHealthPointChanged += (self,last,cur)=>
        {
            int n=self.MaxHealthPoint/10;
            if (last == self.MaxHealthPoint) return;

            int l = last / n;
            int c=cur / n;
            if (l != c)
            {
                if (c >= 3)
                    skill[1].Use();
                else
                    skill[2].Use();
            }
        };
    }

    public override void DoBeforeEntering()
    {
        m_entity.Move(0);
        m_animator.SetFloat("speed", 0.0f);
    }

    public override void Act(MonsterBase npc, Animator ani)
    {
        m_att[0].Reset();
        m_att[1].Reset();
        m_skill[0].Reset();

        if (npc.SkillManager.GetActivateSkill() != null)return;

        if (_status == Status.Attack)
        {
            float r = Random.Range(0, 100);
            if (r < 60)
                m_att[0].Use();
            else if (r >= 60 && r < 80)
                m_att[1].Use();
            else if (r >= 80 && r < 90)
                m_skill[0].Use();
            //else if
        }
    }

    public override void Reason(MonsterBase npc, Animator ani)
    {
        if (npc.SkillManager.GetActivateSkill() != null) return;
        if (Vector3.Distance(playerTransform.position, npc.transform.position) > npc.CanAttackDistance)
        {
            m_att[0].Reset();
            m_att[1].Reset();
            m_skill[0].Reset();
            fsm.PerformTransition(Transition.SeePlayer);
        }
    }
}