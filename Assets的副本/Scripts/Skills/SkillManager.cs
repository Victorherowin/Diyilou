using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill.Context;

namespace Skill
{
    /// <summary>
    /// 除Player外的Entity用
    /// </summary>
    internal class SkillManager
    {
        protected EntityBase m_entity;
        protected Animator m_animator;

        private List<SkillContext> m_skillList=new List<SkillContext>();

        public SkillManager(EntityBase entity, Animator animator)
        {
            m_entity = entity;
            m_animator = animator;
        }

        /// <summary>
        /// 给实体注册技能
        /// </summary>
        /// <typeparam name="T">继承SkillBase的技能类</typeparam>
        public SkillContext RegisterSkill<T>() where T : ISkill, new()
        {
            ISkill skill = new T();
            SkillContext skill_ctx = new SkillContext(m_entity, skill);
            m_skillList.Add(skill_ctx);
            return skill_ctx;
        }

        /// <summary>
        /// 给实体注册蓄力技能
        /// </summary>
        /// <typeparam name="T">继承SkillBase的技能类</typeparam>
        public HoldSkillContext RegisterHoldSkill<T>() where T : ISkill,ISkillHoldable, new()
        {
            ISkill skill = new T();

            HoldSkillContext skill_ctx = new HoldSkillContext(m_entity, skill);

            m_skillList.Add(skill_ctx);
            return skill_ctx;
        }

        /// <summary>
        /// 获取正在使用的技能
        /// </summary>
        /// <returns></returns>
        public ISkill GetActivateSkill()
        {
            if(m_lastSkill!=null)
                return m_lastSkill.Skill;
            return null;
        }

        public SkillContext GetActivateSkillContext()
        {
            AnimatorStateInfo current_status = m_animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo next_status = m_animator.GetNextAnimatorStateInfo(0);

            var ctx=GetSkillContextFromStateInfoInfo(current_status);
            ctx = ctx ?? GetSkillContextFromStateInfoInfo(next_status);
            return ctx;
        }

        public SkillContext GetSkillContextFromStateInfoInfo(AnimatorStateInfo info)
        {
            if (m_skillList == null)
            {
                throw new UnityException("No Registered Skill");
            }

            foreach (var skill_ctx in m_skillList)
            {
                foreach (var name in skill_ctx.Skill.AnimationNames)
                {
                    if (info.IsName(name))
                    {
                        return skill_ctx;
                    }
                }
            }
            return null;
        }

        SkillContext m_lastSkill = null;

        public virtual void Update(EntityStatus currentStatus)
        {
            foreach(var ctx in m_skillList)
            {
                ctx.Update();
            }

            var skill = GetActivateSkillContext();
            if(skill != m_lastSkill)
            {
                if(m_lastSkill!=null)
                    m_lastSkill.Reset();
            }
            m_lastSkill = skill;
        }

        public SkillContext FindContext(ISkill skill)
        {
            foreach(var n in m_skillList)
            {
                if (n.Skill.GetType() == skill.GetType())
                    return n;
            }
            return null;
        }

        public IEnumerator<SkillContext> GetEnumerator()
        {
            return m_skillList.GetEnumerator();
        }
    }
}
