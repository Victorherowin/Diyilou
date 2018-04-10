using Entity;
using Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullet
{

    [RequireComponent(typeof(Rigidbody))]
    internal class BulletBase : MonoBehaviour
    {

        public ISkillLongDistanceable Skill;
        public EntityBase Shooter;

        protected virtual void OnCollisionEnter(Collision other)
        {
            if (Skill != null)
            {
                if (other.collider.tag == "Monster")
                    Skill.OnBulletHit(this, Shooter, other.collider.GetComponentInChildren<EntityBase>());
            }
        }
    }

    static class ShotHelper
    {
        /// <summary>
        /// 射出子弹
        /// </summary>
        /// <param name="shooter">射击者</param>
        /// <param name="bullet_prefab">子弹预制体</param>
        /// <param name="skill">技能</param>
        /// <param name="force">子弹初始力</param>
        public static void Shot(this EntityBase shooter, Vector3 shoot_position, GameObject bullet_prefab,
                                ISkillLongDistanceable skill, Vector3 velocity)
        {
            var obj = GameObject.Instantiate(bullet_prefab, shoot_position, Quaternion.identity);
            var bullet = obj.GetComponent<BulletBase>();
            bullet.Skill = skill;
            bullet.Shooter = shooter;
            bullet.GetComponent<Rigidbody>().velocity= velocity;
        }
    }
}