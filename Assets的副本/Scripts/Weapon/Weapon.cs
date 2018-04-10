using Entity;
using Skill;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    //近距离技能碰撞检测用
    public class Weapon : MonoBehaviour
    {
        private GameObject m_slef;

        private void Start()
        {
            m_slef = GetComponentInParent<EntityBase>().gameObject;
        }


        private void OnTriggerEnter(Collider other)
        {
            EntityBase other_entity = other.GetComponentInChildren<EntityBase>();

            if (other_entity != null&& other_entity.gameObject != m_slef)
            {
                EntityBase self = GetComponentInParent<EntityBase>();
                if (self.HasStatusFlag(EntityStatus.Hold)) return;
                Debug.Log("exec");

                ISkill skill = self.SkillManager.GetActivateSkill();
                if (skill != null)
                {
                    //Debug.Log("enter");
                    skill.OnHit(self, other_entity);
                }
            }
        }
    }
}
