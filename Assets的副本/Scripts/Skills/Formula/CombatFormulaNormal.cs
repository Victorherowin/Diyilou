using Entity;
using Skill.Context;
using System.Collections.Generic;

namespace Skill.Formula
{
    internal class CombatFormulaNormal : IFormula
    {
        private float _tmp_hp_decrease = 0.0f;
        private Dictionary<string, float> args;

        public CombatFormulaNormal(Dictionary<string, float> args)
        {
            this.args = args;
        }

        public int CostShield(EntityBase offense, EntityBase defense, ISkill skill)
        {
            float f;
            f = args.TryGetValue("f", out f) ? f : 0.2f;

            return (int)(_tmp_hp_decrease * f);
        }

        public int TakeDamge(EntityBase offense, EntityBase defense, ISkill skill)
        {
            float a, b, c, d, e;
            a = args.TryGetValue("a", out a) ? a : 1.0f;
            b = args.TryGetValue("b", out b) ? b : 10.0f;
            c = args.TryGetValue("c", out c) ? c : 1.8f;
            d = args.TryGetValue("d", out d) ? d : 0.1f;
            e = args.TryGetValue("e", out e) ? e : 0.2f;

            _tmp_hp_decrease = (a * offense.ATK + skill.Level * b) * c * (1 + (skill.Hit - 1) * d);
            if (defense.Shield == 0)
            {
                return (int)_tmp_hp_decrease;
            }
            else
            {
                return (int)(_tmp_hp_decrease * e);
            }
        }
    }
}