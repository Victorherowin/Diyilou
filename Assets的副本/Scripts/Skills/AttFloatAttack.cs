using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal class AttFloatAttack : SkillBase
    {
        private static readonly string[] s_animationNames = new string[] { "AttFloat1", "AttFloat2", "AttFloat3", "AttFloat4", "AttFloat5", "AttFloat6" };

        private static readonly string[] m_animationNames = new string[]
        {
            "AttFloat1","AttFloat2","AttFloat3","AttFloat4","AttFloat5","AttFloat6",
            "AttFloat1_chg",
            "AttFloat1_bak","AttFloat2_bak","AttFloat3_bak","AttFloat4_bak","AttFloat5_bak","AttFloat6_bak"
        };

        private const int MAX_HIT = 6;

        private int i = 0;

        public AttFloatAttack()
        {
            AnimationNames = m_animationNames;
            CoolingTime = 0.0f;
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            if (i >= MAX_HIT)
            {
                Clear();
            }

            animator.SetTrigger(s_animationNames[i]);
            i++;
            Hit++;
        }

        public override void OnReset(Animator animator)
        {
            foreach (string tri in s_animationNames)
                animator.ResetTrigger(tri);
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
