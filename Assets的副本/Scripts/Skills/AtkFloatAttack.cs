using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal class AtkFloatAttack : SkillBase
    {
        private static readonly string[] s_animationNames = new string[] { "AtkFloat1", "AtkFloat2", "AtkFloat3", "AtkFloat4"};

        private static readonly string[] m_animationNames = new string[]
        {
            "AtkFloat1","AtkFloat2","AtkFloat3","AtkFloat4",
            "AtkFloat1_bak","AtkFloat2_bak","AtkFloat3_bak","AtkFloat4_bak"
        };

        private const int MAX_HIT = 4;

        private int i = 0;

        public AtkFloatAttack()
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
