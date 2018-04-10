using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{

    public class TriggerSkillEffect : StateMachineBehaviour
    {
        public float Delay = 0.0f;
        public string MountPath="Effect";
        public GameObject Effect;
        public bool AutoDestroy = false;
        private Transform m_mount_node;

        private bool _flag = false;

        private GameObject _instance;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(m_mount_node==null)
            {
                m_mount_node = animator.transform.Find(MountPath);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!_flag&&stateInfo.normalizedTime > Delay)
            {
                _instance = GameObject.Instantiate(Effect, m_mount_node);
                _flag = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if(AutoDestroy)
            {
                GameObject.Destroy(_instance,1.5f);
            }
            _flag = false;
        }
    }
}