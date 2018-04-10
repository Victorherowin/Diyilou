using Attributes;
using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Skill.Context
{
    /// <summary>
    /// 隐藏一些技能状态的中间类
    /// </summary>
    internal class SkillContext
    {
        public ISkill Skill { get; protected set; }
        public EntityBase Self { get; set; }

        [SerializeField, ReadOnly]
        protected float m_remainCoolingTime;
        public float RemainCoolingTime { get { return m_remainCoolingTime; } }

        public Func<SkillContext,bool> UseCond= s=> true;

        protected Animator m_animator;

        public SkillContext(EntityBase self, ISkill skill)
        {
            Self = self;
            Skill = skill;
            m_animator = self.GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            if(m_remainCoolingTime>0.0f)
            {
                m_remainCoolingTime -= Time.deltaTime;
            }
        }

        public virtual void Use()
        {
            if (m_remainCoolingTime > 0) return;

            Skill.OnUse(Self,m_animator);

            m_remainCoolingTime = Skill.CoolingTime;
        }

        public virtual void Reset()
        {
            Skill.OnReset(m_animator);
        }
    };
}
