using Entity;
using Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Skill.Monster.BossQ01c
{
    class BossAtt01 : SkillBase
    {
        private static string[] s_animiation_names = new string[] {
            "A_att01","B_att01"
        };//动画状态机中对应技能的动画名

        public BossAtt01()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)//击中目标时调用
        {
            ApplyFormula(self, target);
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
