using Entity;
using Entity.Monster;
using Other;
using Skill.Context;
using UnityEngine;

 class AttackState : FSMState
{
    
    private Status _status = Status.Attack_1;

    private enum Status
    {
        Idle,
        Attack_1,
        Skill
    }
    

    private Vector3 tall = new Vector3(0, 2, 0);
    private Transform playerTransform;
    private Animator m_animator;
    private float dir;
    private CharacterController m_controller;

    private SkillContext m_att,m_skill;

    public AttackState(FSMSystem fsm, GameObject obj,SkillContext att,SkillContext skill) : base(fsm)
    {
        m_animator = obj.GetComponent<Animator>();
        m_controller = obj.GetComponent<CharacterController>();
        stateID = StateID.Attack;
        playerTransform = GameObject.Find("Player").transform;
        m_att = att;
        m_skill = skill;
    }

    private float _timer=0.0f;

	public override void Act(MonsterBase npc, Animator ani)
    {
        ani.SetFloat("speed", 0.0f);
        npc.Move(0);
        if (npc.SkillManager.GetActivateSkill() != null) return;
        dir = Vector3.Distance(npc.transform.position, playerTransform.transform.position);

        if (_status == Status.Attack_1)
        {
			if (dir < npc.CanAttackDistance)
            {
                m_att.Use();
                //monster.TakeDamage(3);
            }
            _status = Status.Idle;
        }
        else if (_status == Status.Skill)
        {
			if (dir < npc.CanAttackDistance)
            {
                m_skill.Use();
                //ani.SetTrigger("skill");
                //EntityBase monster = npc.GetComponent<EntityBase>();
                //monster.TakeDamage(5);
            }
            _status = Status.Idle;
        }
        else if(_status == Status.Idle)
        {
            if (_timer > npc.IdleTime)
            {
                _timer = 0.0f;
                int r = Random.Range(0, 100);
                if (r > 70)
                    _status = Status.Attack_1;
                else
                    _status = Status.Skill;
            }
            _timer += Time.fixedDeltaTime;
        }
    }

    public override void Reason(MonsterBase npc, Animator ani)
    {
        if (npc.SkillManager.GetActivateSkill() != null) return;
        if (Vector3.Distance(playerTransform.position, npc.transform.position) > npc.CanAttackDistance)
        {
            var o = npc.transform.position + new Vector3(0, 0.8f, 0);
            var t = playerTransform.position + new Vector3(0, 0.5f, 0);
            Ray lookRay = new Ray(o, (t - o).normalized);
            //Ray lookRay = new Ray(npc.transform.position + tall, npc.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(lookRay, out hit, 10, Layers.Player | Layers.Building))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.DrawRay(npc.transform.position + tall, npc.transform.forward, Color.red, 5f);
                    fsm.PerformTransition(Transition.SeePlayer);
                }
            }
            else
                fsm.PerformTransition(Transition.LostPlayer);
        }
    }
}