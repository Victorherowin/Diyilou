using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;

namespace Skill.Formula
{
    internal interface IFormula
    {
        int TakeDamge(EntityBase offense, EntityBase defense,ISkill skill);
        int CostShield(EntityBase offense, EntityBase defense,ISkill skill);
    }
}

