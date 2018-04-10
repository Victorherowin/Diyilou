using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class PlaySound : StateMachineBehaviour
    {
        public AudioClip Clip;

        private AudioSource m_source;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(m_source==null)
            {
                m_source = animator.GetComponent<AudioSource>();
            }
            m_source.PlayOneShot(Clip,1.0f);
        }
    }
}
