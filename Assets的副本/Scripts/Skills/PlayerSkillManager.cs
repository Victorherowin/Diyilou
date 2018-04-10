using Entity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using Skill.Context;
using Shoot.Trajectory;
using Manager;

namespace Skill
{
    internal enum StatusMatchOp
    {
        Not,One,And,Equal
    }

    internal struct SkillBinder
    {
        public string[] Keys;
        public SkillContext Skill;
        public EntityStatus Status;
        public StatusMatchOp Op;
        public bool Remote;
        public string RemoteKey;
    }

    /// <summary>
    /// Player用
    /// </summary>
    internal class PlayerSkillManager : SkillManager
    {
        #region 技能容器
        //技能,注册的键
        private LinkedList<SkillBinder> m_skillList = new LinkedList<SkillBinder>();
        private LinkedList<SkillBinder> m_comboSkillList = new LinkedList<SkillBinder>();
        private LinkedList<SkillBinder> m_holdSkillList = new LinkedList<SkillBinder>();

        private Dictionary<string, bool> m_keyDownList = new Dictionary<string, bool>();
        #endregion

        private bool m_shootMode = false;
        public bool ShootMode { get { return m_shootMode; } }

        private static PlayerSkillManager m_instance;
        public static PlayerSkillManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        public static void ResetInstance()
        {
            m_instance = null;
        }

        private float m_velocityFactor = 1.0f;

        private Transform m_shootLineTransform;
        private GameObject m_shootLine;
        private ShootTrajectory m_trajectory;

        public PlayerSkillManager(EntityBase entity, Animator animator):base(entity,animator)
        {
            if (m_instance == null)
                m_instance = this;
            else
                throw new UnityException("PlayerSkillManager.Instance!=null");

            m_shootLineTransform = m_entity.transform.Find("ShotLine");
            m_shootLine = m_shootLineTransform.gameObject;
            m_trajectory = m_entity.GetComponentInChildren<ShootTrajectory>();
            m_camera_trace=Camera.main.GetComponent<CameraPlayerTrace>();
        }

        /// <summary>
        /// 给Player注册技能
        /// </summary>
        /// <typeparam name="T">继承SkillBase的技能类</typeparam>
        /// <param name="key">指定按键</param>
        public SkillContext BindSkill<T>(string key, EntityStatus status = EntityStatus.Grounded, 
            StatusMatchOp op = StatusMatchOp.One,bool is_remote= false,string remote_key= null) where T : ISkill,new()
        {
            var skill_ctx=base.RegisterSkill<T>();
            
            SkillBinder binder;
            binder.Keys = new string[] { key };
            binder.Skill = skill_ctx;
            binder.Status = status;
            binder.Op = op;
            binder.Remote = is_remote;
            binder.RemoteKey = remote_key;

            m_skillList.AddLast(binder);
            m_keyDownList[key] = false;
            return skill_ctx;
        }

        /// <summary>
        /// 绑定技能到指定组合按键(注册组合技)
        /// </summary>
        /// <typeparam name="T">技能</typeparam>
        /// <param name="combo">指定按键Combo.例如JJKJ</param>
        /// <returns></returns>
        public SkillContext BindComboSkill<T>(string[] combo, EntityStatus status = EntityStatus.Grounded, 
            StatusMatchOp op = StatusMatchOp.One) where T : ISkill,new()
        {
            if (combo.Length == 0)
                throw new ArgumentException("combo.Length==0");

            var skill_ctx = base.RegisterSkill<T>();

            SkillBinder binder;
            binder.Keys = combo;
            binder.Skill = skill_ctx;
            binder.Status = status;
            binder.Op = op;
            binder.Remote = false;
            binder.RemoteKey = null;

            m_comboSkillList.AddLast(binder);
            foreach(var k in combo)
            {
                m_keyDownList[k] = false;
            }
            return skill_ctx;
        }

        /// <summary>
        /// 绑定蓄力技
        /// </summary>
        /// <typeparam name="T">技能</typeparam>
        /// <param name="key">指定按键</param>
        /// <returns></returns>
        public HoldSkillContext BindHoldSkill<T>(string key, EntityStatus status = EntityStatus.Grounded, 
            StatusMatchOp op = StatusMatchOp.One,bool is_remote=false,string remote_key=null) where T : ISkill,ISkillHoldable,new()
        {
            var skill_ctx = base.RegisterHoldSkill<T>();
            SkillBinder binder;
            binder.Keys = new string[] { key };
            binder.Skill = skill_ctx;
            binder.Status = status;
            binder.Op = op;
            binder.Remote = is_remote;
            binder.RemoteKey = remote_key;

            m_holdSkillList.AddLast(binder);
            m_keyDownList[key] = false;
            m_holdTime[key] = s_holdTime;
            return skill_ctx;
        }

        #region Hold Variable
        private List<string> m_currentKeys=new List<string>();
        private const float s_clearTime= 0.3f;
        private const float s_holdTime = 0.1f;
        private float m_clear_time = 0.0f;
        private Dictionary<string,float> m_holdTime = new Dictionary<string, float>();
        #endregion

        private SkillBinder[] FindBinder(IEnumerable<SkillBinder> v,string key)
        {
            List<SkillBinder> r = new List<SkillBinder>();
            foreach(var b in v)
            {
                if (b.Keys[0] == key)
                    r.Add(b);
            }
            return r.ToArray();
        }

        private void CheckKeySkillUse(EntityStatus currentStatus)
        {
            var all_key = new List<string>(m_keyDownList.Keys);

            foreach (var key in all_key)
            {
                if (InputManager.GetButtonDown(key))
                {
                    SkillBinder[] binders;
                    m_clear_time = s_clearTime;

                    binders=FindBinder(m_skillList, key);
                    //普通技
                    foreach(var binder in binders)
                    {
                        if (binder.Skill.UseCond(binder.Skill) && CanUse(binder, currentStatus))
                        {
                            binder.Skill.Use();
                            break;
                        }
                    }

                    //Combo技
                    m_currentKeys.Add(key);
                    var combo = m_currentKeys.ToArray();

                    foreach (var binder_pair in m_comboSkillList)
                    {
                        if (ComboEquals(combo, binder_pair.Keys))
                        {
                            if (binder_pair.Skill.UseCond(binder_pair.Skill) && CanUse(binder_pair, currentStatus))
                            {
                                binder_pair.Skill.Use();
                                m_currentKeys.Clear();
                                break;
                            }
                        }
                    }

                    m_keyDownList[key] = true;
                }
            }

            foreach (var binder_pair in m_holdSkillList)
            {
                if (Input.GetButton(binder_pair.Keys[0]))
                {
                    //蓄力技(蓄力)
                    if (m_holdTime[binder_pair.Keys[0]] <= 0.0f)
                    {
                        if (binder_pair.Skill.UseCond(binder_pair.Skill) && CanUse(binder_pair, currentStatus))
                        {
                            (binder_pair.Skill as HoldSkillContext).Hold();
                            break;
                        }
                    }
                    else
                    {
                        m_holdTime[binder_pair.Keys[0]] -= Time.deltaTime;
                    }
                }
            }

            foreach (var key in all_key)
            {
                if (InputManager.GetButtonUp(key))
                {
                    SkillBinder[] binders;

                    binders = FindBinder(m_holdSkillList, key);
                    //蓄力技(释放)
                    foreach(var binder in binders)
                    {
                        if (m_holdTime[key] <= 0.0f)
                        {
                            if (binder.Skill.UseCond(binder.Skill) && CanUse(binder, currentStatus))
                            {
                                binder.Skill.Use();
                                break;
                            }
                        }
                        m_holdTime[key] = s_holdTime;
                    }
                    m_keyDownList[key] = false;
                }
            }

            if (m_clear_time <= 0.0f)
            {
                m_currentKeys.Clear();
            }
            else
            {
                m_clear_time -= Time.deltaTime;
            }

            if (m_currentKeys.Count >= 5)
                m_currentKeys.Clear();
        }

        private void CheckKeyRemoteEnable()
        {
            foreach (var binder in m_holdSkillList)
            {
                if (binder.RemoteKey == null) continue;

                if (InputManager.GetButtonDown(binder.RemoteKey))
                {
                    float v = (binder.Skill.Skill as ISkillLongDistanceable).GetInitialVelocity();
                    EnableShootMode(v);
                }
            }

            foreach (var binder in m_skillList)
            {
                if (binder.RemoteKey == null) continue;

                if (InputManager.GetButtonDown(binder.RemoteKey))
                {
                    float v = (binder.Skill.Skill as ISkillLongDistanceable).GetInitialVelocity();
                    EnableShootMode(v);
                }
            }
        }
        private void DisplayShootLine()
        {
            ShootTrajectory.DrawTrajectory(m_entity, m_velocityFactor);
            var cam = Camera.main;
            var screen_pos = cam.WorldToScreenPoint(m_shootLineTransform.position);
            var dir = (screen_pos-Input.mousePosition);
            dir.z = 0;
            dir.Normalize();

            //float a = Vector3.Dot(m_entity.transform.forward, m_entity.transform.parent.forward);


            //if (Vector3.Dot(a>0?Vector3.left:Vector3.right,dir)<=0.0f)
            //{
            //    if (Vector3.Dot(Vector3.up, dir) > Vector3.Dot(Vector3.down, dir))
            //        dir = Vector3.up;
            //    else
            //        dir = Vector3.down;
            //}

           dir = cam.transform.TransformDirection(dir);

            m_shootLineTransform.LookAt(dir + m_shootLineTransform.position);
        }

        public override void Update(EntityStatus currentStatus)
        {
            base.Update(currentStatus);

            if (ShootMode && InputManager.GetButtonDown("DisableRemote"))
            {
                DisableShootMode();
                return;
            }

            CheckKeyRemoteEnable();
            if(ShootMode)DisplayShootLine();
            CheckKeySkillUse(currentStatus);
        }

        private CameraPlayerTrace m_camera_trace;
        private Vector3 _old_camera_offset;

        /// <summary>
        /// 打开远程模式
        /// </summary>
        /// <param name="v">初速度的倍数</param>
        public void EnableShootMode(float v)
        {
            if (ShootMode) return;

            m_velocityFactor = v;
            m_shootMode = true;
            m_shootLine.transform.localEulerAngles = Vector3.zero;
            m_shootLine.SetActive(true);

            _old_camera_offset=m_camera_trace.CAMERA_POSITION_OFFSET;
            m_camera_trace.CAMERA_POSITION_OFFSET = new Vector3(14.6f,7.6f,0.0f);
        }
        public void DisableShootMode()
        {
            if (!ShootMode) return;

            m_velocityFactor = 1.0f;
            m_shootMode = false;
            //m_shootLine.transform.localEulerAngles = Vector3.zero;
            m_shootLine.SetActive(false);

            m_camera_trace.CAMERA_POSITION_OFFSET=_old_camera_offset;
        }

        #region private method
        private bool CanUse(SkillBinder binder,EntityStatus currentStatus)
        {
            if (binder.Remote != m_shootMode) return false;

            switch(binder.Op)
            {
                case StatusMatchOp.One:
                    return StatusCompare(binder.Status, currentStatus);
                case StatusMatchOp.Equal:
                    return binder.Status == currentStatus;
                case StatusMatchOp.And:
                    return (currentStatus & binder.Status) == binder.Status;
                case StatusMatchOp.Not:
                    return StatusCompare(binder.Status, ~currentStatus);
            }
            return false;
        }

        private bool StatusCompare(EntityStatus a, EntityStatus b)
        {
            if (a == EntityStatus.Movement && b == EntityStatus.Stand) return true;
            if (b == EntityStatus.Movement && a == EntityStatus.Stand) return true;
            if (a == EntityStatus.Stand && b == EntityStatus.Stand) return true;

            uint ua=(uint)a, ub=(uint)b;
            while(ua!=0&&ub!=0)
            {
                if((ua & 0x1)!=0&&
                   (ub & 0x1) != 0&&
                   (ua&0x1)==(ub&0x1))
                {
                    return true;
                }
                ua >>= 1;
                ub >>= 1;
            }
            return false;
        }

        private bool ComboEquals(string[] a,string [] b)
        {
            if (a.Length == 0 || b.Length == 0) return false;
            if (a.Length != b.Length) return false;

            for(int i=0;i<a.Length;i++)
            {
                if(a[i]!=b[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}