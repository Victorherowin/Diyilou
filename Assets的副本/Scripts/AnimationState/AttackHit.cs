using Entity;
using Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Other;

namespace StateMachineBehaviours.Attack
{
    class AttackHit : StateMachineBehaviour
    {
        [Serializable]
        public class HitSetting
        {
            public string MountPoint="Effect";
            public GameObject HitEffect;
            public float TimePoint;
            public Vector3 Impact = Vector3.zero;
            [NonSerialized]
            public GameObject hit_effect_instance;
        }

        public List<HitSetting> HitSettings;

        private int _trigger_num = 0;

        public float Length=2.5f;
        public float Angle = 30.0f;
        public Vector3 Direction = Vector3.forward;

        private float m_angleCos;

        private EntityBase m_entity=null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(m_entity == null)
            {
                m_entity = animator.GetComponent<EntityBase>();
                m_angleCos = Mathf.Cos(Angle);
                HitSettings.Add(new HitSetting() { TimePoint = 1.0f });
                Direction.Normalize();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            float t = animatorStateInfo.normalizedTime;
            if (_trigger_num < (HitSettings.Count-1)&&t>= HitSettings[_trigger_num].TimePoint&& t < HitSettings[_trigger_num+1].TimePoint)
            {
                var setting = HitSettings[_trigger_num];
                Collider[] all = Physics.OverlapSphere(m_entity.transform.position, Length,Layers.Player|Layers.Monster);
                foreach(var c in all)
                {
                    Vector3 v= c.transform.position - m_entity.transform.position;
                    Vector3 dir = m_entity.transform.TransformDirection(Direction);

                    float a = Vector3.Dot(v.normalized, dir);

                    if (a>m_angleCos&&v.magnitude<Length)
                    {
                        ISkill skill = m_entity.SkillManager.GetActivateSkill();
                        var target = c.GetComponentInChildren<EntityBase>();
                        if (target != null && skill != null)
                        {
                            if (m_entity.ParentTransform.tag != target.ParentTransform.tag)
                            {
                                target.SetImpact(setting.Impact);
                                if (setting.HitEffect != null)
                                {
                                    var mount = target.transform.Find(setting.MountPoint);
                                    setting.hit_effect_instance = GameObject.Instantiate(setting.HitEffect, mount);
                                }
                                skill.OnHit(m_entity, target);
                            }
                        }
                    }
                }
                _trigger_num++;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _trigger_num = 0;
            foreach(var e in HitSettings)
            {
                GameObject.Destroy(e.hit_effect_instance,1.0f);
            }
        }
    }
}


