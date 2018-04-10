using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class LieFloor : StateMachineBehaviour
    {
        private EntityBase m_entity;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(m_entity == null)
            {
                m_entity = animator.GetComponent<EntityBase>();
            }
            m_entity.SetStatusFlag(EntityStatus.Floor);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            m_entity.ResetStatusFlag(EntityStatus.Floor);
        }
    }
}
