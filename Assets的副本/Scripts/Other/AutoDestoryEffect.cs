using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestoryEffect : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < transform.childCount; i++)
        {
            var ps = transform.GetChild(i).GetComponentsInChildren<ParticleSystem>();
            Transform t=null;
            foreach (var p in ps)
            {
                if (!p.isStopped)
                {
                    t = null;
                    break;
                }
                t = p.transform;
            }
            if (t != null)
            {
                while (t.parent.name != "Effect")
                    t = t.parent;
                GameObject.Destroy(t.gameObject);
            }
        }
	}
}
