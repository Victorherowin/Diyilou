using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using Skill;

namespace Skill.Monster.BossQ01c
{
    class BossSkill01 : SkillBase
    {

        private static string[] s_animiation_names = new string[] {
            "A_skill01_chg","A_skill01_act","A_skill01_bak",
            "B_skill01_chg","B_skill01_act","B_skill01_bak"
        };//动画状态机中对应技能的动画名

        public BossSkill01()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)//击中目标时调用
        {
            ApplyFormula(self, target);
        }

        public override void OnReset(Animator ani)//重置动画状态机参数
        {
            ani.ResetTrigger("skill01");

        }

        public override void OnUse(EntityBase self, Animator ani)//设置动画状态机参数
        {
            ani.SetTrigger("skill01");
        }
    }
}