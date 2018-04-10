using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Entity;
//using Skill;
//using Skill.Context;

//namespace Monster
//{
//    internal class MonsterHuangRong : EntityBase
//    {
//        private Animator m_anim;
//        private int m_hitNum = 0;
//        private AudioSource source;

//        private SkillContext m_normalAttack;

//        // Use this for initialization
//        void Start()
//        {
//            m_anim = GetComponent<Animator>();
//            source = GetComponentInChildren<AudioSource>();

//           // m_normalAttack=SkillManager.RegisterSkill<NormalAtack>();

//            OnStatusChanged += (last, cur) =>
//             {
//                 if (base.HasStatusFlag(EntityStatus.TakeHit))
//                 {
//                     Debug.Log("Hit");
//                     if (m_hitNum < 3)
//                     {
//                         m_anim.Play("hurt01");
//                     }
//                     else
//                     {
//                         source.Play();
//                         m_anim.Play("hurt02_1", 0, 0.2f);
//                     }
//                     m_hitNum = m_hitNum++ % 3;
//                 }
//             };

//          //  OnHurt += (hp) => { };
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            m_normalAttack.Use();
//        }
//    }
//}
