using Attributes;
using Game.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Monster
{
    class MonsterBase : EntityBase
    {
        public AudioClip RunSoundClip;

        public float MoveSpeed = 5.0f;
        public float IdleTime = 0.7f;
        public float CanAttackDistance = 1.5f;
        public float SeePlayerDistance = 5f;
        public float ChaseLostDistance = 10f;

        public PathNode StartNode;
        public PathNode EndNode;

        protected FSMSystem FSM;
        private bool _stop_fsm = false;
        [ReadOnly]
        public string _fsm_status_debug;

        private AudioSource m_source;

        private Material m_mainMaterial;

        private float _fade_step = 0.05f;

        private static readonly string[] s_hurt_animations = new string[] { "hurt1", "hurt2", "hurt3", "hurt4", "hurt5", "hurt6", "hurt7" };
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
            OnHurt += (self, hp) =>
              {
                  var dir = (Player.PlayerEntity.Instance.transform.position - transform.position).normalized;
                  if (Vector3.Dot(dir, transform.forward) < 0.0f)
                      self.transform.LookAt(transform.position - transform.forward);//受击转向，有BUG

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
                      else if(!(info.IsName("hurt5")|| info.IsName("hurt6")|| info.IsName("hurt7")))
                      {
                          m_animator.SetTrigger("hurt5");
                      }
                      return;
                  }
                  else if(self.Impact.y<-0.5)
                  {
                      m_animator.SetTrigger("hurt7");
                  }

                  // z∈(0.0,0.5] hurt1
                  // z∈(0.5,1.0] hurt2
                  // z∈(1.0,1.5] hurt3
                  if (GameMath.InRange(self.Impact.z,0.0f,0.5f))
                  {
                      m_animator.SetTrigger("hurt1");
                      return;
                  }
                  else if (GameMath.InRange(self.Impact.z, 0.5f, 1.0f))
                  {
                      m_animator.SetTrigger("hurt2");
                      return;
                  }
                  else if (GameMath.InRange(self.Impact.z,1.0f,1.5f))
                  {
                      m_animator.SetTrigger("hurt3");
                      return;
                  }
              };

            OnDead += (self) =>
             {
                 ResetAnimator(m_animator);

                 m_animator.SetTrigger("dead");
                 _stop_fsm = true;
                 InvokeRepeating("DeadFade",1.5f,_fade_step);
             };
            m_mainMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
            FSM = new FSMSystem(this.gameObject);
            m_source = GetComponent<AudioSource>();
        }

        private static void ResetAnimator(Animator ani)
        {
            foreach (var p in ani.parameters)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        ani.SetBool(p.name, false);
                        break;
                    case AnimatorControllerParameterType.Float:
                        ani.SetFloat(p.name, 0.0f);
                        break;
                    case AnimatorControllerParameterType.Int:
                        ani.SetInteger(p.name, 0);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        ani.ResetTrigger(p.name);
                        break;
                }
            }
        }

        protected override void Start()
        {
            ParentTransform.position = new Vector3(StartNode.transform.position.x,ParentTransform.position.y, StartNode.transform.position.z);
            var path=PathManager.Instance.FindPath(StartNode,EndNode);
            if (path.Length > 1)
            {
                var link=path[0].GetLink(path[1]);
                transform.LookAt(transform.position + link.NextNodeDir);
            }
            base.Start();
        }

        private void DeadFade()
        {
            if (m_mainMaterial.color.a > 0.0f)
                m_mainMaterial.color = new Color(m_mainMaterial.color.r, m_mainMaterial.color.g, m_mainMaterial.color.b, m_mainMaterial.color.a - _fade_step);
            else
                Destroy(ParentTransform.gameObject);
        }

        protected override void Update()
        {
            _fsm_status_debug = FSM.CurrentState.ToString();
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if(!_stop_fsm)
                FSM.Update(this.transform.parent.gameObject, m_animator);
        }

        public override void OnPassNode(PathNode node)
        {
            float a = 0.0f;

            foreach (var link in node.ConnectLinks)
            {
                float cos = Vector3.Dot(link.NextNodeDir, transform.forward);
                if (cos > a)
                {
                    a = cos;
                    SelectLink = link;
                }
            }
        }

        private void PlayRunSound()
        {
            m_source.PlayOneShot(RunSoundClip, 1.0f);
        }
    }

    static class GameMath
    {
        public static bool InRange(float x,float a,float b)
        {
            return x>a && x < b;
        }
    }
}
