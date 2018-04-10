using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    internal interface ISkill
    {
        /// <summary>
        /// AnimationName与Animation节点名保存一致
        /// </summary>
        string[] AnimationNames { get; }
        float CoolingTime { get; }
        int Level { get; }
        int Hit { get; }
       
        void OnReset(Animator ani);
        void OnUse(EntityBase self, Animator ani);
        void OnHit(EntityBase self, EntityBase target);
  
    }
}
