using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill;
using Entity;
using Bullet;
using Player;
using Shoot.Trajectory;

namespace Skill
{
    internal class RemoteNormalSkill : SkillBase,ISkillLongDistanceable,ISkillHoldable
    {
        private const float m_force = 100;

        public RemoteNormalSkill()
        {
            AnimationNames = new string[] { "RemoteNormalSkill" };
            CoolingTime = 1.0f;
        }

        public void OnHoldEnter(EntityBase self, Animator ani)
        {
            var shotline = self.transform.Find("ShotLine").gameObject;
            shotline.transform.localEulerAngles = Vector3.zero;

            shotline.SetActive(true);
            self.GetComponent<PlayerMove>().enabled = false;
            foreach (var p in ani.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Bool)
                    ani.SetBool(p.name, false);
                if (p.type == AnimatorControllerParameterType.Float)
                    ani.SetFloat(p.name, 0.0f);
                if (p.type == AnimatorControllerParameterType.Int)
                    ani.SetInteger(p.name, 0);
                if (p.type == AnimatorControllerParameterType.Trigger)
                    ani.ResetTrigger(p.name);
            }
            ShowTrajectory(self);
        }

        public void ShowTrajectory(EntityBase self)
        {
            Vector3 pos = self.transform.position;
            pos.y += 3.8f;
            Vector3 dir = self.transform.Find("ShotLine/Sphere").position - pos;
            dir.Normalize();

            ShootTrajectory t = self.GetComponentInChildren<ShootTrajectory>();
            t.SetInitialVelocity(dir * m_force);
            t.ShowTrajectory(pos);
        }

        private readonly Vector3 c_asix = new Vector3(-1, 0, 0);

        public void OnHold(EntityBase self, Animator ani)
        {
            var shotline = self.transform.Find("ShotLine");
            float x = Input.GetAxisRaw("Vertical");

            float a;
            Vector3 asix;
            shotline.transform.localRotation.ToAngleAxis(out a, out asix);
            a *= asix.x;

            if (a <= -90 && x > 0) return;
            if (a >= 90 && x < 0) return;

            if(x!=0)
                ShowTrajectory(self);

            shotline.transform.Rotate(c_asix, x * 5);
        }

        public void OnHoldExit(EntityBase self, Animator ani)
        {
            var shotline = self.transform.Find("ShotLine").gameObject;
            shotline.SetActive(false);
            self.GetComponent<PlayerMove>().enabled = true;
        }

        public void OnBulletHit(BulletBase bullet,EntityBase self, EntityBase target)
        {
            Vector3 dir = bullet.transform.position - target.transform.position;
            if (Vector3.Dot(target.transform.forward, dir) > 0)
            {
                target.TakeDamage(10);
            }
            else
            {
                target.TakeDamage(15);
            }
            GameObject.Destroy(bullet.gameObject);
        }

        public override void OnReset(Animator ani)
        {
            //TODO
        }

        public override void OnUse(EntityBase self, Animator ani)
        {
            Vector3 pos = self.transform.position;
            pos.y += 3.8f;
            Vector3 dir = self.transform.Find("ShotLine/Sphere").position - pos;

            dir.Normalize();
            self.Shot(pos + dir, Resources.Load("Prefabs/Bullet/Test/TestBullet") as GameObject, this, dir * m_force);
        }

        public float GetInitialVelocity()
        {
            return m_force;
        }
    }
}
