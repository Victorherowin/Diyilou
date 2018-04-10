using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Context
{
    internal class HoldSkillContext : SkillContext
    {
        public HoldSkillContext(EntityBase self, ISkill skill) : base(self, skill)
        {
            if (skill is ISkillHoldable)
            {
                Skill = skill;
            }
            else
            {
                throw new UnityException(string.Format("{0} not found OnHold()", skill));
            }
        }

        private bool m_hold_enter = false;
        public void Hold()
        {
            if (m_remainCoolingTime > 0) return;

            Self.SetStatusFlag(EntityStatus.Hold);

            if(!m_hold_enter)
            {
                (Skill as ISkillHoldable).OnHoldEnter(Self, m_animator);
                m_hold_enter = true;
            }

            (Skill as ISkillHoldable).OnHold(Self, m_animator);
        }

        public override void Use()
        {
            base.Use();
            (Skill as ISkillHoldable).OnHoldExit(Self, m_animator);
            Self.ResetStatusFlag(EntityStatus.Hold);
        }

        public override void Reset()
        {
            base.Reset();
            m_hold_enter = false;
        }
    }
}
