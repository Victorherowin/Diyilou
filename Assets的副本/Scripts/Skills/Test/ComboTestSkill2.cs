using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using Skill;

namespace Skill.Test
{
    internal class ComboTestSkill2 : SkillBase
    {
        public ComboTestSkill2()
        {
            AnimationNames = new string[] { "ComboAttack2" };
            CoolingTime = 1.0f;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self, target);
        }

        public override void OnReset(Animator animator)
        {
            animator.ResetTrigger("ComboTestAttack2");
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            animator.SetTrigger("ComboTestAttack2");
        }
    }
}
