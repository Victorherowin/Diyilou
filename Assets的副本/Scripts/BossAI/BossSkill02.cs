using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using Skill;
using Entity.Monster;

namespace Skill.Monster.BossQ01c
{
    class BossSkill02 : SkillBase
    {

        private static string[] s_animiation_names = new string[] {
            "A_skill02_chg","A_skill02_act","A_skill02_bak",
            "B_skill02_chg","B_skill02_act","B_skill02_bak"
        };//动画状态机中对应技能的动画名

        public BossSkill02()
        {
            AnimationNames = s_animiation_names;
        }

        public override void OnHit(EntityBase self, EntityBase target)//击中目标时调用
        {
            ApplyFormula(self, target);
        }

        public override void OnReset(Animator ani)//重置动画状态机参数
        {
            ani.ResetTrigger("SwitchStatus");
        }

        public override void OnUse(EntityBase self, Animator ani)//设置动画状态机参数
        {
            ani.SetTrigger("SwitchStatus");
            bool isb = ani.GetBool("IsBStatus");
            ani.SetBool("IsBStatus",!isb);

            var boss = (self as BossEnemy);
            if (!isb)
                boss.AudioSource.clip = boss.StatusBStandSound;
            else
                boss.AudioSource.clip = boss.StatusAStandSound;
            boss.AudioSource.Play();
        }
    }
}