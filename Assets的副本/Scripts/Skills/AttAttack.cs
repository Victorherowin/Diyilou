using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal class AttAttack : SkillBase
    {
        private static readonly string[] s_animationNames = new string[] { "AttAttack1", "AttAttack2", "AttAttack3", "AttAttack4", "AttAttack5" };

        private string[] m_animationNames = new string[]
        {
            "AttAttack1","AttAttack2","AttAttack3","AttAttack4","AttAttack5",
            "AttAttack1_chg","AttAttack2_chg","AttAttack3_chg","AttAttack4_chg",
            "AttAttack1_bak","AttAttack2_bak","AttAttack3_bak","AttAttack4_bak","AttAttack5_bak",
        };
        private int i = 0;

        public AttAttack()
        {
            AnimationNames = m_animationNames;
            CoolingTime = 0.0f;
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            if (i >= 5)
            {
                Clear();
            }

            animator.SetTrigger(s_animationNames[i] + "Tri");
            Hit++;
            i++;
        }

        public override void OnReset(Animator animator)
        {
            foreach (string tri in s_animationNames)
                animator.ResetTrigger(tri + "Tri");
            i = 0;
        }

        private void Clear()
        {
            i = 0;
            Hit = 1;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self, target);
        }
    }
}
