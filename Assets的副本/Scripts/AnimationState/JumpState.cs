using Entity;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{

    public class JumpState : StateMachineBehaviour
    {
        public float JumpHeight = 1.0f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            EntityBase player = animator.GetComponent<EntityBase>();

            player.Jump(JumpHeight);
        }
    }
}
