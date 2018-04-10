using UnityEngine;
using System.Collections;

namespace Other.Utility.Effect
{
    [SerializeField]
    public class Delay : MonoBehaviour
    {

        public float delayTime = 1.0f;
        public float duration = 0;

        // Use this for initialization
        void Start()
        {
            gameObject.SetActive(false);
            Invoke("DelayFunc", delayTime);
        }

        void DelayFunc()
        {
            gameObject.SetActive(true);
            if (duration > 0)
                GameObject.Destroy(gameObject, duration);
        }

    }
}
