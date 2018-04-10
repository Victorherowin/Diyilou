using Attributes;
using Entity;
using Game.Path;
using Manager;
using Skill;
using Skill.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    internal class PlayerEntity : EntityBase
    {
        [ReadOnly]
        public string StatusDebug;
        [ReadOnly]
        public string SkillDebug;

        public static PlayerEntity Instance;
        public AudioClip RunSoundClip;

        private AudioSource m_source;

        private void OnEnable()
        {
            if (Instance != null)
                throw new UnityException("PlayerEntity.Instance!=null");
            Instance = this;
        }

        private void OnDisable()
        {
            Instance = null;
        }

        //单例
        private PlayerSkillManager m_playerSkillManager;

        private static readonly string[] s_hurt_animations = new string[] { "hurt1", "hurt2", "hurt3", "hurt4", "hurt5", "hurt6", "hurt7"};
        protected override string[] HurtAnimationNames
        {
            get
            {
                return s_hurt_animations;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_source = GetComponent<AudioSource>();
        }

        // Use this for initialization
        protected override void Start()
        {
            m_animator = GetComponent<Animator>();
            m_playerSkillManager = new PlayerSkillManager(this, m_animator);
            SkillManager = m_playerSkillManager;

            PlayerSkillManager.Instance.BindSkill<AttFloatAttack>("Touch").UseCond=(s)=> s.Skill.Hit == 1 ? InputManager.GetButton("FloatSkillTrigger") : true;
            PlayerSkillManager.Instance.BindSkill<AttAttack>("Touch");
            PlayerSkillManager.Instance.BindSkill<AtkFloatAttack>("Thump").UseCond = (s) => s.Skill.Hit == 1 ? InputManager.GetButton("FloatSkillTrigger") : true;
            PlayerSkillManager.Instance.BindSkill<AtkAttack>("Thump");
            //PlayerSkillManager.Instance.BindHoldSkill<HoldTestSkill>("Thump");
            PlayerSkillManager.Instance.BindComboSkill<ComboTestSkill>(new string[] { "Thump", "Thump","Touch", "Thump" },EntityStatus.SecondJump);
            PlayerSkillManager.Instance.BindComboSkill<ComboTestSkill2>(new string[] { "Forward", "Touch", "Touch" },EntityStatus.Grounded);
            //PlayerSkillManager.Instance.BindHoldSkill<RemoteNormalSkill>("NormalShot");
            PlayerSkillManager.Instance.BindSkill<TransferSkillPart1>("Touch", EntityStatus.Movement|EntityStatus.Grounded,StatusMatchOp.One,true, "TransferShot");
            //PlayerSkillManager.Instance.BindSkill<TransferSkillPart2>("Transfer");
            //PlayerSkillManager.Instance.BindHoldSkill<ProjectileSkill>("ProjectileShot"); 

            base.OnHurt += (self, cur) =>
              {
                  if (base.HasStatusFlag(EntityStatus.TakeHit))
                  {
                      if (self.Impact.y > 0.5f)//浮空控制(丑，但没办法了)
                      {
                          var info = m_animator.GetCurrentAnimatorStateInfo(0);
                          if (info.IsName("hurt5"))
                          {
                              if (info.normalizedTime > 0.58f)
                              {
                                  m_animator.SetTrigger("hurt7");
                              }
                              else
                              {
                                  m_animator.SetTrigger("hurt6");
                              }
                          }
                          else if (!(info.IsName("hurt5") || info.IsName("hurt6") || info.IsName("hurt7")))
                          {
                              m_animator.SetTrigger("hurt5");
                          }
                          return;
                      }
                      else if (self.Impact.y < -0.5)
                      {
                          m_animator.SetTrigger("hurt7");
                          return;
                      }

                      if (!base.HasStatusFlag(EntityStatus.Grounded))
                          m_animator.SetTrigger("hurt3");
                      else
                          m_animator.SetTrigger("hurt1");
                  }
              };

            OnDead += (self) =>
              {
                  m_animator.SetTrigger("die");
                  //InputManager.EnableAsix = false;
                  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
              };

            base.Start();
        }

        protected override void OnDestroy()
        {
            PlayerSkillManager.ResetInstance();
            Instance = null;
            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();
            StatusDebug = Status.ToString();

            var k = PlayerSkillManager.Instance.GetActivateSkill();
            if (k != null)
                SkillDebug = k.ToString();
            else
                SkillDebug = "";

            if(k!=null || HasStatusFlag(EntityStatus.Hurting))
            {
                InputManager.EnableAsix = false;
            }
            else if(IsSurvival)
            {
                InputManager.EnableAsix = true;
            }
        }

        public override void OnPassNode(PathNode node)
        {
            if (LastPassNode == null) return;

            Vector3 t = transform.forward;
            float angle = 0.0f;

            var offset = new Vector3(0, InputManager.GetAxisRaw("Vertical"), 0);
            if (HasStatusFlag(EntityStatus.FirstJump) || HasStatusFlag(EntityStatus.SecondJump))
                offset = base.Velocity;
            offset.Normalize();

            PathNode.PathLink next_link= null;

            foreach (var next in LastPassNode.ConnectLinks)
            {
                if (next.NextNode == LastPassNode) continue;

                var vec = next.NextNodeDir;
                var dir = vec.normalized;

                float a = Vector3.Dot(dir, (t + offset).normalized);

                if (a > angle)
                {
                    angle = a;
                    t = dir;
                    next_link = next;
                }
            }

            if (next_link != null)
            {
                SelectLink = next_link;
            }
            else
            {
                foreach (var next in LastPassNode.ConnectLinks)
                {
                    if (next.NextNode != LastPassNode)
                    {
                        SelectLink = next;
                        break;
                    }
                }
            }
        }

        private void PlayRunSound()
        {
            m_source.PlayOneShot(RunSoundClip,1.0f);
        }
    }
}
