using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Skill.Monster
{
    class MonsterSkill1Attack : SkillBase
    {
        private static string[] s_animiation_names = new string[] { "skill01"};

        public MonsterSkill1Attack()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self, target);
        }

        public override void OnReset(Animator ani)
        {
            ani.ResetTrigger("skill");
        }

        public override void OnUse(EntityBase self, Animator ani)
        {
            ani.SetTrigger("skill");
        }
    }
}
