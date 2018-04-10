using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using Skill;
using Entity.Monster;

namespace Skill.Monster.BossQ01c
{
    class BossSkill03 : SkillBase
    {

        private static string[] s_animiation_names = new string[] {
            "A_skill03"
        };//动画状态机中对应技能的动画名

        public BossSkill03()
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
            ani.SetBool("IsRage", true);
            ani.SetBool("IsBStatus",true);

            var boss = (self as BossEnemy);
            boss.AudioSource.clip = boss.StatusBStandSound;
            boss.AudioSource.Play();
        }
    }
}