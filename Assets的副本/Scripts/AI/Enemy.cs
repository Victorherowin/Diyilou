using Game.Path;
using Skill.Context;
using Skill.Monster;
using UnityEngine;

namespace Entity.Monster
{
    internal class Enemy : MonsterBase
    {
        private SkillContext m_attContext;
        private SkillContext m_skillContext;

        protected override void Start()
        {
            m_attContext=SkillManager.RegisterSkill<MonsterAttAttack>();
            m_skillContext = SkillManager.RegisterSkill<MonsterSkill1Attack>();

            InitFSM();

            base.Start();
        }

        private void InitFSM()
        {
            FSMState patrolState = new PatrolState(FSM, this, StartNode, EndNode);
            FSM.AddState(patrolState);
            patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);
			FSMState chaseState = new ChaseState(FSM, this.gameObject);
            FSM.AddState(chaseState);
            chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);
            FSMState attackState = new AttackState(FSM, this.gameObject,m_attContext,m_skillContext);
            FSM.AddState(attackState);
            chaseState.AddTransition(Transition.CanAttack, StateID.Attack);
            attackState.AddTransition(Transition.SeePlayer, StateID.Chase);
            attackState.AddTransition(Transition.LostPlayer, StateID.Patrol);
        }
    }
}