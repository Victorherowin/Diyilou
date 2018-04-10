using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Skill.Monster
{
    class MonsterAttAttack : SkillBase
    {
        private static string[] s_animiation_names = new string[] { "attack01" };

        public MonsterAttAttack()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)
        {
            ApplyFormula(self,target);
        }

        public override void OnReset(Animator ani)
        {
            ani.ResetTrigger("Attack_1");
          
        }

        public override void OnUse(EntityBase self, Animator ani)
        {
            ani.SetTrigger("Attack_1");
           
        }
    }
}
