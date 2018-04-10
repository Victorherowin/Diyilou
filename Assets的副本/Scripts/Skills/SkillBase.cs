using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System;
using Skill.Formula;

namespace Skill
{
    internal abstract class SkillBase : ISkill
    {
        private string[] m_animation_names;
        public string[] AnimationNames {
            get { return m_animation_names; }
            protected set { m_animation_names = value; }
        }

        private float m_coolint_time = 0.0f;
        public float CoolingTime {
            get { return m_coolint_time; }
            protected set { m_coolint_time = value; }
        }

        private int m_hit=1;
        public int Hit {
            get { return m_hit; }
            protected set { m_hit = value; }
        }

        private int m_level = 1;
        public int Level {
            get { return m_level; }
            protected set { m_level = value; }
        }

        private IFormula m_formula;

        public void SetFormula(IFormula formula)
        {
            m_formula = formula;
        }

        protected void ApplyFormula(EntityBase self, EntityBase target)
        {
            int h=m_formula.TakeDamge(self, target, this);
            int s=m_formula.CostShield(self, target, this);
            target.TakeDamage(h);
            target.CostShield(s);
        }

        public virtual void OnHit(EntityBase self, EntityBase target) { throw new NotImplementedException(); }
        public abstract void OnReset(Animator ani);
        public abstract void OnUse(EntityBase self, Animator ani);
    }
}
