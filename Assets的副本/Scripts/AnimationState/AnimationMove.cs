using Entity;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class AnimationMove : StateMachineBehaviour
    {
        private bool _is_negative_speed = false;
        public float MoveTime = 0.0f;
        public float StopTime = 1.0f;
        public float MoveDistance = 0.0f;
        public bool Turn = true;

        private float _moveDurationTime;

        private float _speed;

        private EntityBase entity = null;
        private CharacterController cc = null;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (entity == null)
            {
                entity = animator.transform.GetComponent<EntityBase>();
                cc = entity.transform.parent.GetComponent<CharacterController>();
                _moveDurationTime = animatorStateInfo.length* (StopTime - MoveTime);
                _speed = MoveDistance / _moveDurationTime;
            }

            _is_negative_speed = _speed < 0?true:false;

            if (animatorStateInfo.normalizedTime >= MoveTime&& animatorStateInfo.normalizedTime<StopTime)
            {
                if (_is_negative_speed)
                {
                    entity.Move(_speed, 1,Turn);
                }
                else
                {
                    entity.Move(_speed * Vector3.Dot(entity.transform.forward, entity.ParentTransform.forward), 1);
                }
            }
            else
            {
                entity.Move(0,1);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            entity.Move(0,1);
        }
    }
}