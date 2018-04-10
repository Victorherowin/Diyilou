using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bullet
{
    internal class Shot1Bullet : BulletBase
	{
        public int ReflectCount=5;
        public float DestroyDistance = 20f;

        private int m_currentReflectCount = 0;

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            m_currentReflectCount++;
            if (m_currentReflectCount >= ReflectCount)
            {
                GameObject.Destroy(gameObject);
            }

            if(Vector3.Distance(transform.position,Player.PlayerEntity.Instance.transform.position)>= DestroyDistance)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}