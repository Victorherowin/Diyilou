using Entity;
using Manager;
using UnityEngine;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private PlayerEntity m_player;

        private Transform m_meshTransform;

        public float Speed = 5.0f;
        public float Gravity = 9.8F;
        public float InputRecoverTime = 0.2f;
        public Vector3 circlePoint;
        public bool OnCircle = false;
        public bool opposite = false;

        private CharacterController m_controller;
        private Animator m_animator;

        private void Awake()
        {
            m_player = GetComponentInChildren<PlayerEntity>();
            m_meshTransform = transform.GetChild(0);
            m_controller = GetComponent<CharacterController>();
            m_animator = GetComponentInChildren<Animator>();

            m_player.OnStatusChanged += (l, c) =>
            {
                if (!EntityBase.HasStatusFlag(l, EntityStatus.Grounded) &&
                  EntityBase.HasStatusFlag(c, EntityStatus.Grounded))
                {
                    InputManager.Enable = false;
                    Invoke("EnableInput", InputRecoverTime);
                }

                if (!EntityBase.HasStatusFlag(l, EntityStatus.Grounded) && EntityBase.HasStatusFlag(c, EntityStatus.Grounded))
                {
                    m_player.ResetStatusFlag(EntityStatus.FirstJump);
                    m_player.ResetStatusFlag(EntityStatus.SecondJump);

                    m_animator.SetBool("jump02", false);
                    m_animator.SetBool("jump01", false);
                }

                if (EntityBase.HasStatusFlag(c, EntityStatus.Forward | EntityStatus.Grounded))
                    m_animator.SetBool("move", true);
                else
                    m_animator.SetBool("move", false);
            };
        }

        private void EnableInput()
        {
            InputManager.Enable = true;
        }

        private void Update()
        {
            if (OnCircle)
            {
                Vector3 dir1 = Vector3.Normalize(transform.position - circlePoint);
                Vector3 pos1 = transform.position + dir1 * 20;

                float x = (pos1.x - transform.position.x) * Mathf.Cos(-Mathf.PI / 2) + (pos1.z - transform.position.z) * Mathf.Sin(-Mathf.PI / 2) + transform.position.x;
                float z = -(pos1.x - transform.position.x) * Mathf.Sin(-Mathf.PI / 2) + (pos1.z - transform.position.z) * Mathf.Cos(-Mathf.PI / 2) + transform.position.z;
                Vector3 dir2 = new Vector3(x, transform.position.y, z) - transform.position;
                transform.LookAt(new Vector3(x, transform.position.y, z));
                //m_moveVelocity = new Vector3(InputManager.GetAxisRaw("Horizontal") * Mathf.Cos(angle), 0, -InputManager.GetAxisRaw("Horizontal") * Mathf.Sin(angle));
            }

            //Apply Move
            int i = 1;
            if (opposite)
                i = -1;

            m_player.Move(InputManager.GetAxisRaw("Horizontal") * Speed*i);

            if (m_player.HasStatusFlag(EntityStatus.Grounded))
            {
                if (InputManager.GetButtonDown("Jump"))
                {
                    m_player.SetStatusFlag(EntityStatus.FirstJump);
                    m_animator.SetBool("jump01", true);
                }
            }
            else
            {
                if (m_player.HasStatusFlag(EntityStatus.FirstJump) && InputManager.GetButtonDown("Jump"))
                {
                    m_player.ResetStatusFlag(EntityStatus.FirstJump);
                    m_player.SetStatusFlag(EntityStatus.SecondJump);

                    m_animator.SetBool("jump02", true);
                    m_animator.SetBool("jump01", false);
                }
            }
        }
    }
}