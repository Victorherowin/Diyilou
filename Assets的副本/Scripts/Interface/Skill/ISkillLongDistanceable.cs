using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;

namespace Skill
{
    internal interface ISkillLongDistanceable
    {
        float GetInitialVelocity();

        /// <summary>
        /// 子弹碰到怪物时被调用
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="target"></param>
        void OnBulletHit(BulletBase bullet,EntityBase self,EntityBase target);
    }
}
