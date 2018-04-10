using Bullet;
using Entity;
using Player;
using UnityEngine;
using Shoot.Trajectory;

namespace Skill
{
    internal class TransferSkillPart1 : SkillBase, ISkillLongDistanceable
    {

        public TransferSkillPart1()
        {
            CoolingTime = 0.5f;
            AnimationNames = new string[] { "TransferShot" };
        }

        private const float m_force = 35.0f;

        public void OnBulletHit(BulletBase bullet, EntityBase self, EntityBase target)
        {
            /*Vector3 dir = bullet.transform.position - target.transform.position;

            var transfer_target = GameObject.Find("TransferTarget(Clone)");
            if (transfer_target != null)
                GameObject.Destroy(transfer_target);

            var t=GameObject.Instantiate(Resources.Load("Prefabs/Skill/TransferTarget"), target.transform);
            */

            Vector3 t = self.ParentTransform.position;
            self.ParentTransform.position = target.ParentTransform.position;
            target.ParentTransform.position = t;

            GameObject.Destroy(bullet.gameObject);
        }

        private readonly Vector3 c_asix = new Vector3(-1, 0, 0);

        public override void OnReset(Animator ani)
        {
        }

        public override void OnUse(EntityBase self, Animator ani)
        {
            Vector3 pos = self.transform.Find("ShotLine").position;
            Vector3 dir = self.transform.Find("ShotLine/Sphere").position - pos;

            dir.Normalize();
            self.Shot(pos+dir, Resources.Load("Prefabs/Bullet/FireBullet") as GameObject, this, dir * m_force);

            ani.SetTrigger("TransferShotTri");
        }

        public float GetInitialVelocity()
        {
            return m_force;
        }
    }

    internal class TransferSkillPart2 : SkillBase
    {
        public TransferSkillPart2()
        {
            AnimationNames = new string[] { "TransferSkillPart2" };
            CoolingTime = 0.0f;
        }

        public override void OnReset(Animator ani)
        {
            return;
        }

        public override void OnUse(EntityBase self, Animator ani)
        {
            var transfer_target = GameObject.Find("TransferTarget(Clone)");
            if (transfer_target == null)
                return;

            Vector3 t = self.transform.position;
            self.transform.position = transfer_target.transform.parent.position;
            transfer_target.transform.parent.position = t;

            GameObject.Destroy(transfer_target);
        }
    }
}