using Game.Path;
using Skill.Context;
using Skill.Monster;
using Skill.Monster.BossQ01c;
using UnityEngine;
using System;

namespace Entity.Monster
{
	internal class BossEnemy : MonsterBase
	{
		private SkillContext[] m_attContext=new SkillContext[2];
		private SkillContext[] m_skillContext=new SkillContext[3];

        [NonSerialized]
        public AudioSource AudioSource;

        public AudioClip StatusAStandSound, StatusBStandSound;

        protected override void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.clip = StatusAStandSound;
            base.Awake();
        }

        protected override void Start()
		{
			m_attContext[0] = SkillManager.RegisterSkill<BossAtt01>();
            m_attContext[1] = SkillManager.RegisterSkill<BossAtt02>();
            m_skillContext[0] = SkillManager.RegisterSkill<BossSkill01>();
            m_skillContext[1] = SkillManager.RegisterSkill<BossSkill02>();
            m_skillContext[2] = SkillManager.RegisterSkill<BossSkill03>();

            InitFSM();

			base.Start();
		}

		private void InitFSM()
		{
			FSMState chaseState = new ChaseState(FSM, this.gameObject);
			FSM.AddState(chaseState);
			FSMState BossAttackState = new BossAttackState(FSM, this, m_attContext, m_skillContext);
			FSM.AddState(BossAttackState);
			chaseState.AddTransition(Transition.CanAttack, StateID.Attack);
            BossAttackState.AddTransition(Transition.SeePlayer, StateID.Chase);
            //BossAttackState.AddTransition(Transition.LostPlayer, StateID.Patrol);
		}

        private void ShakeCamera()
        {
            Camera.main.GetComponent<ShakeCamera>().shake();
        }
	}
}
