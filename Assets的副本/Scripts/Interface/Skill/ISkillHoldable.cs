using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal interface ISkillHoldable : ISkill
    {
        void OnHoldEnter(EntityBase self, Animator ani);
        void OnHold(EntityBase self, Animator ani);
        void OnHoldExit(EntityBase self, Animator ani);
    }
}
