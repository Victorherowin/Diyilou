using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class BulletTime : StateMachineBehaviour
    {
        public float TimeScala = 0.5f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Time.timeScale = TimeScala;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Time.timeScale = 1.0f;
        }
    }
}
