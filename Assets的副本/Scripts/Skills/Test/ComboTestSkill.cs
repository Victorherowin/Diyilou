using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill;

namespace Skill.Test
{
    internal class ComboTestSkill : SkillBase
    {
        public ComboTestSkill()
        {
            AnimationNames = new string[] { "ComboAttack" };
            CoolingTime = 1.0f;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self, target);
        }

        public override void OnUse(EntityBase self, Animator animator)
        {
            Debug.Log(this.GetType());
            animator.SetTrigger("TestSkillTrigger");
        }

        public override void OnReset(Animator animator)
        {
            animator.ResetTrigger("TestSkillTrigger");
        }
    }
}
