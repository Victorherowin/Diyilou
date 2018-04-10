using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{

    internal class HoldTestSkill : SkillBase,ISkillHoldable
    {
        public HoldTestSkill()
        {
            AnimationNames = new string[] { "HoldAttack" };
            CoolingTime = 1.0f;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self, target);
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            m_isHeld = false;
            animator.SetTrigger("HoldAttack");
            animator.SetBool("Hold", false);
        }

        private bool m_isHeld = false;
        public void OnHold(EntityBase self, Animator animator)
        {
        }

        public void OnHoldEnter(EntityBase self, Animator ani)
        {
            if (!m_isHeld)
            {
                ani.SetBool("Hold", true);
                m_isHeld = true;
            }
        }

        public override void OnReset(Animator animator)
        {
            animator.ResetTrigger("HoldAttack");
            animator.SetBool("Hold", false);
        }

        public void OnHoldExit(EntityBase self, Animator ani)
        {

        }
    }
}
