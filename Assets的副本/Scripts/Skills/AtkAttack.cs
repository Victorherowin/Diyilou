using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal class AtkAttack : SkillBase
    {
        private static readonly string[] s_animationNames = new string[] { "AtkAttack1", "AtkAttack2", "AtkAttack3" };

        private string[] m_animationNames = new string[9] 
        {
            "AtkAttack1","AtkAttack2","AtkAttack3",
            "AtkAttack1_chg","AtkAttack2_chg","AtkAttack3_chg",
            "AtkAttack1_bak","AtkAttack2_bak","AtkAttack3_bak"
        };
        private int i = 0;

        public AtkAttack()
        {
            AnimationNames = m_animationNames;
            CoolingTime = 0.0f;
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            if (i >= 3)
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
            Clear();
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
