using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class ResetAnimatorTrigger : StateMachineBehaviour
    {
        public List<string> TriggerNames;

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            foreach(var s in TriggerNames)
            {
                animator.ResetTrigger(s);
            }
        }
    }
}
