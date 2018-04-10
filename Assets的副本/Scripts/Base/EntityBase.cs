using Attributes;
using Game.Path;
using Skill;
using Skill.Formula;
using Skill.Formula.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Entity
{
    [Flags]
    internal enum EntityStatus : uint
    {
        Stand = 0u,
        Forward = 1u << 0,
        Backward = 1u << 1,
        Sprint = 1u << 2,

        //Midair
        FirstJump = 1u << 3,

        SecondJump = 1u << 4,

        Downed = 1u << 5,
        TakeHit = 1u << 6,
        Hold = 1u << 7,

        Hurting = 1u << 8,//受伤
        Floor = 1u<<9,//躺在地上

        Grounded = 1u << 31,
        Midair = FirstJump | SecondJump,
        Movement = Stand | Sprint | Backward | Forward | Midair
    }

    [RequireComponent(typeof(Animator))]
    internal abstract class EntityBase : MonoBehaviour
    {
        public int MaxHealthPoint = 100;
        public int MaxShield = 100;

        private SkillManager m_skillManager;

        public SkillManager SkillManager
        {
            get { return m_skillManager; }
            protected set { m_skillManager = value; }
        }

        [SerializeField, ReadOnly]
        private bool m_isSurvival = true;

        public bool IsSurvival { get { return m_isSurvival; } }

        [SerializeField, ReadOnly]
        private EntityStatus m_status = EntityStatus.Stand;

        public EntityStatus Status { get { return m_status; } }

        #region Attribute

        [SerializeField, ReadOnly]
        private int m_healthPoint;

        public int HealthPoint { get { return m_healthPoint; } }

        [SerializeField, ReadOnly]
        private int m_shield;

        public int Shield { get { return m_shield; } }

        public int ATK = 1;

        public PathNode LastPassNode = null;
        public PathNode.PathLink SelectLink = null;
        [NonSerialized]
        public Transform ParentTransform;

        private Vector3 m_impactForce;
        public Vector3 Impact { get { return m_impactForce; } }

        #endregion Attribute

        #region Event

        public delegate void OnHealthPointChangedEvt(EntityBase self, int last_hp, int hp);

        public event OnHealthPointChangedEvt OnHealthPointChanged;

        public delegate void OnShieldChangedEvt(EntityBase self, int last_shield, int Shield);

        public event OnShieldChangedEvt OnShieldChanged;

        public delegate void OnHurtEvt(EntityBase self, int hp);

        public event OnHurtEvt OnHurt;

        public delegate void OnRecoverEvt(EntityBase self, int hp);

        public event OnRecoverEvt OnRecover;

        public delegate void OnDeadEvt(EntityBase self);

        public event OnDeadEvt OnDead;

        public delegate void OnStatusChangedEvt(EntityStatus lastStatus, EntityStatus currentStatus);

        public event OnStatusChangedEvt OnStatusChanged;

        #endregion Event

        protected abstract string[] HurtAnimationNames { get; }

        protected Animator m_animator;
        protected CharacterController m_controller;
        public static LinkedList<EntityBase> AllEntitys = new LinkedList<EntityBase>();

        public bool IsGrounded
        {
            get
            {
                return m_controller.isGrounded;
            }
        }

        protected virtual void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_controller = GetComponentInParent<CharacterController>();
            m_skillManager = new SkillManager(this, m_animator);
            ParentTransform = transform.parent;

            m_healthPoint = MaxHealthPoint;
            m_shield = MaxShield;

            AllEntitys.AddLast(this);

            OnHealthPointChanged += (self, last_hp, hp) =>
            {
                if (last_hp < hp && OnRecover != null)
                    OnRecover(self, hp);
                else if (last_hp > hp && OnHurt != null)
                    OnHurt(self, hp);

                if (hp == 0)
                {
                    m_isSurvival = false;
                    if (OnDead != null)
                    {
                        OnDead(self);
                    }
                }
            };

            /*OnStatusChanged += (l, c) =>
            {
                if (!HasStatusFlag(l, EntityStatus.Grounded) && HasStatusFlag(c, EntityStatus.Grounded))
                        LastPassNode = PathManager.Instance.FindNearNode(this);
            };*/
        }

        protected virtual void OnDestroy()
        {
            AllEntitys.Remove(this);
        }

        #region 初始化技能公式

        protected virtual void Start()
        {
            if (!Directory.Exists("json"))
                Directory.CreateDirectory("json");
            string json_file = "json/" + GetType().Name + ".json";

            if (!File.Exists(json_file))
            {
                FormulaConfigCollection tmp = new FormulaConfigCollection();
                tmp.SkillList = new List<FormulaConfigJson>();
                foreach (var skill in SkillManager)
                {
                    tmp.SkillList.Add(new FormulaConfigJson()
                    {
                        SkillName = skill.Skill.GetType().Name,
                        FormulaName = "CombatFormulaNormal",
                        Arguments = new List<ArgumentPair>()
                    });
                }
                File.WriteAllText(json_file, JsonUtility.ToJson(tmp, true));
            }

            FormulaConfigCollection obj = JsonUtility.FromJson<FormulaConfigCollection>(File.ReadAllText(json_file));
            foreach (var skill in SkillManager)
            {
                foreach (var arg in obj.SkillList)
                {
                    if (arg.SkillName == skill.Skill.GetType().Name)
                    {
                        Dictionary<string, float> dic = new Dictionary<string, float>();
                        foreach (var a in arg.Arguments)
                        {
                            dic.Add(a.Var, a.Value);
                        }
                        var f = Activator.CreateInstance(Type.GetType("Skill.Formula." + arg.FormulaName), dic) as IFormula;
                        (skill.Skill as SkillBase).SetFormula(f);
                    }
                }
            }
        }

        #endregion 初始化技能公式

        /// <summary>
        /// 恢复实体HealthPoint
        /// </summary>
        /// <param name="hp">如果hp+HealthPoint大于MaxHealthPoint则HealthPoint=MaxHealthPoint</param>
        public void RecoverHealthPoint(int hp)
        {
            int new_hp = Mathf.Min(m_healthPoint + hp, MaxHealthPoint);
            if (new_hp != m_healthPoint)
                if (OnHealthPointChanged != null)
                    OnHealthPointChanged(this, m_healthPoint, new_hp);
            m_healthPoint = new_hp;
        }

        /// <summary>
        /// 恢复实体HealthPoint值到hp
        /// </summary>
        /// <param name="hp">如果hp大于MaxHealthPoint则HealthPoint=MaxHealthPoint</param>
        public void RecoverHealthPointTo(int hp)
        {
            int new_hp = Mathf.Min(hp, MaxHealthPoint);
            if (new_hp != m_healthPoint)
                if (OnHealthPointChanged != null)
                    OnHealthPointChanged(this, m_healthPoint, new_hp);
            m_healthPoint = new_hp;
        }

        /// <summary>
        /// 恢复实体Shield
        /// </summary>
        /// <param name="hp">如果hp+HealthPoint大于MaxHealthPoint则HealthPoint=MaxHealthPoint</param>
        public void RecoverShieldPoint(int shield)
        {
            int new_shield = Mathf.Min(m_shield + shield, MaxShield);
            if (new_shield != m_shield)
                if (OnShieldChanged != null)
                    OnShieldChanged(this, m_shield, new_shield);
            m_shield = new_shield;
        }

        /// <summary>
        /// 恢复实体HealthPoint值到hp
        /// </summary>
        /// <param name="hp">如果hp大于MaxHealthPoint则HealthPoint=MaxHealthPoint</param>
        public void RecoverShieldPointTo(int shield)
        {
            int new_shield = Mathf.Min(shield, MaxShield);
            if (new_shield != m_shield)
                if (OnShieldChanged != null)
                    OnShieldChanged(this, m_shield, new_shield);
            m_shield = new_shield;
        }

        /// <summary>
        /// 消耗实体Shield
        /// </summary>
        /// <param name="shield">消耗的Shield</param>
        public void CostShield(int shield)
        {
            int new_shield = Mathf.Max(m_shield - shield, 0);
            if (new_shield != m_shield)
                if (OnShieldChanged != null)
                    OnShieldChanged(this, m_shield, new_shield);
            m_shield = new_shield;
        }

        /// <summary>
        /// 造成伤害,如果在无敌时间内则不造成伤害,每次成功造成伤害
        /// 会设置由InvincibleTime指定的无敌时间
        /// </summary>
        /// <param name="damage">造成的伤害</param>
        public void TakeDamage(int damage)
        {
            SetStatusFlag(EntityStatus.TakeHit);

            int new_hp = Mathf.Max(0, m_healthPoint - damage);
            if (new_hp != m_healthPoint)
                if (OnHealthPointChanged != null)
                    OnHealthPointChanged(this, m_healthPoint, new_hp);
            m_healthPoint = new_hp;
            ResetStatusFlag(EntityStatus.TakeHit);
        }

        public void SetStatusFlag(EntityStatus status)
        {
            var old_status = m_status;
            m_status |= status;
            if (m_status != old_status)
                if (OnStatusChanged != null)
                    OnStatusChanged(old_status, m_status);
        }

        public void ResetStatusFlag(EntityStatus status)
        {
            var old_status = m_status;
            m_status &= ~status;
            if (m_status != old_status)
                if (OnStatusChanged != null)
                    OnStatusChanged(old_status, m_status);
        }

        public void ClearStatusFlag()
        {
            if (OnStatusChanged != null)
                OnStatusChanged(m_status, EntityStatus.Stand);
            m_status = EntityStatus.Stand;
        }

        public bool HasStatusFlag(EntityStatus status)
        {
            return (m_status & status) == status;
        }

        public static bool HasStatusFlag(EntityStatus src, EntityStatus status)
        {
            return (src & status) == status;
        }

        private int m_velocityPriority = 0;
        private Vector3 m_moveVelocity = Vector3.zero;
        private Vector3 m_jumpVelocity = Vector3.zero;
        private Vector3 m_gravityVelocity = Vector3.zero;
        private Vector3 m_Velocity = Vector3.zero;

        protected Vector3 Velocity { get { return m_Velocity; } }

        public float Gravity = 30.0f;

        private int m_groundCount = 0;
        private const int MAX_COUNT = 5;

        protected virtual void Update()
        {
            SkillManager.Update(Status);
            m_impactForce = Vector3.zero;

            m_gravityVelocity += Vector3.down * Gravity * Time.deltaTime;
            m_Velocity = m_moveVelocity + m_jumpVelocity;

            m_controller.Move((m_Velocity + m_gravityVelocity) * Time.deltaTime);

            //if (SelectLink.CurveFunc != null&&m_moveVelocity!=Vector3.zero)
            //{
            //    float t = LastPassNode.GetTFormPosition(this, SelectLink.NextNode);
            //    var p = SelectLink.CurveFunc(t);
            //    p.y = ParentTransform.position.y;
            //    ParentTransform.position = p;
            //}

            if (m_controller.isGrounded)
            {
                m_groundCount = 0;
                m_jumpVelocity = Vector3.zero;
                m_gravityVelocity = Vector3.zero;
                SetStatusFlag(EntityStatus.Grounded);
            }
            else
            {
                //去除下坡时的落地状态抖动
                if (m_groundCount == MAX_COUNT)
                {
                    ResetStatusFlag(EntityStatus.Grounded);
                    m_groundCount = 0;
                }
                m_groundCount++;
            }

            if (CheckHurtStatus())
                SetStatusFlag(EntityStatus.Hurting);
            else
                ResetStatusFlag(EntityStatus.Hurting);
        }

        private bool CheckHurtStatus()
        {
            foreach(var s in HurtAnimationNames)
            {
                var info=m_animator.GetCurrentAnimatorStateInfo(0);
                if (info.IsName(s))
                    return true;

                info = m_animator.GetNextAnimatorStateInfo(0);
                if (info.IsName(s))
                    return true;
            }
            return false;
        }

        protected virtual void FixedUpdate()
        {

        }

        public void Jump(float h)
        {
            m_gravityVelocity = Vector3.zero;
            m_jumpVelocity = Vector3.up * Mathf.Sqrt(2 * h * Gravity);
            ResetStatusFlag(EntityStatus.Grounded);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="x">米每秒</param>
        public void Move(float x,int priority = 0, bool turn = true)
        {
            float absx = Mathf.Abs(x);

            #region 模型方向控制
            if(turn)
                transform.LookAt(transform.position + ParentTransform.forward * x);

            #endregion 模型方向控制

            #region Entity状态操作

            if (absx > 0.5)
                SetStatusFlag(EntityStatus.Forward);
            else
                ResetStatusFlag(EntityStatus.Forward);

            #endregion Entity状态操作

            Vector3 dir = ParentTransform.forward;

            if (LastPassNode != null && SelectLink != null)
                dir = SelectLink.DirectionFunc(this);
            dir.y = 0;
            dir.Normalize();

            //根据节点方向改变Entity方向
            if (Vector3.Dot(dir, ParentTransform.forward) >= 0)
                ParentTransform.LookAt(ParentTransform.position + dir);
            else
                ParentTransform.LookAt(ParentTransform.position - dir);

            if (priority<m_velocityPriority) return;

            if (absx <= 10e-6)
            {
                m_velocityPriority = 0;
                m_moveVelocity = Vector3.zero;
            }
            else
            {
                m_velocityPriority = priority;
                if (turn)
                    m_moveVelocity = transform.forward * absx;
                else
                    m_moveVelocity = transform.forward * x;
            }
        }

        /// <summary>
        /// 设置冲击力(下一帧清除)
        /// </summary>
        /// <param name="a"></param>
        public void SetImpact(Vector3 a)
        {
            m_impactForce = a;
        }

        public virtual void OnPassNode(PathNode node) { }
    }
}