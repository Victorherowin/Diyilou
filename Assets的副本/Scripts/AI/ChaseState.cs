using Entity;
using Entity.Monster;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class ChaseState : FSMState
{  
    private EntityBase m_player;
    private EntityBase m_self;
    private Transform playerTransform;
    private Animator m_animator;

    private CharacterController m_controller;
	public ChaseState(FSMSystem fsm, GameObject obj) : base(fsm)
    {
        m_animator = obj.GetComponent<Animator>();
        m_controller = obj.GetComponentInParent<CharacterController>();
        m_self = obj.GetComponent<EntityBase>();
        stateID = StateID.Chase;        
        playerTransform = GameObject.Find("Player").transform;

    }

    private enum Status
    {
        Chase,
      
    }

    private float _dirFactor = 1.0f;

    public override void Act(MonsterBase npc, Animator ani)
    {
        if (npc.SkillManager.GetActivateSkill() != null) return;
        if (npc.HasStatusFlag(EntityStatus.Hurting)) return;
		if(Vector3.Distance(playerTransform.position, npc.transform.position)>= npc.CanAttackDistance)
        {
            var dir = (playerTransform.position-npc.ParentTransform.position).normalized;
            if (Vector3.Dot(dir, npc.ParentTransform.forward) < 0.0f)
                _dirFactor = -1.0f;
            else
                _dirFactor = 1.0f;
            npc.Move(npc.MoveSpeed*_dirFactor);

            ani.SetFloat("speed", 1.0f);
        }
	}

    public override void Reason(MonsterBase npc, Animator ani)
    {
        //if (_status !=Status.Chase) return;      
		if (Vector3.Distance(playerTransform.position, npc.ParentTransform.position) > npc.ChaseLostDistance)
        {
            fsm.PerformTransition(Transition.LostPlayer);
            //ani.SetFloat("speed", 0.0f);
        }
        if (Vector3.Distance(playerTransform.position, npc.ParentTransform.position) < npc.CanAttackDistance)
        {
            fsm.PerformTransition(Transition.CanAttack);
        }
    }
}