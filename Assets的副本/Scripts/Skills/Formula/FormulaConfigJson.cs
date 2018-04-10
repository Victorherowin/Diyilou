using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Formula.Json
{
    [Serializable]
    internal class FormulaConfigCollection
    {
        public List<FormulaConfigJson> SkillList;
    }

    [Serializable]
    internal class ArgumentPair
    {
        public string Var;
        public float Value;
    }

    [Serializable]
    internal class FormulaConfigJson
    {
        public string SkillName;
        public string FormulaName;
        public List<ArgumentPair> Arguments;
    }
}
