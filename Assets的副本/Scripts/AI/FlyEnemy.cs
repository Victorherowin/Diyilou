using Entity;
using Game.Path;
using Skill.Context;
using Skill.Monster;
using UnityEngine;

namespace Entity.Monster
{
    internal class FlyEnemy : MonsterBase
    {
        private SkillContext m_att;

        // Use this for initialization
        protected override void Start()
        {

            m_att = SkillManager.RegisterSkill<FlyMonsterAttAttack>();

            FSMState patrolState = new PatrolState(FSM, this, StartNode, EndNode);
			FSM.AddState(patrolState);
			patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);
			FSMState chaseState = new ChaseState(FSM, this.gameObject);
			FSM.AddState(chaseState);
			chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);
			FSMState attackState = new FlyAttackState(FSM, this.gameObject,m_att);
			FSM.AddState(attackState);
			chaseState.AddTransition(Transition.CanAttack, StateID.Attack);
			attackState.AddTransition(Transition.SeePlayer, StateID.Chase);

            base.Start();
        }
    }
}