using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour {
    private GameObject selfPlayer;
    private Animator anim;
    private int timer = 0;
    private AnimatorStateInfo animInfo;
    private int hitNum=0;
    private GameObject player;
    private AudioSource source;
	// Use this for initialization
	void Start () {
        selfPlayer = gameObject.transform.parent.parent.gameObject;
        anim = selfPlayer.GetComponent<Animator>();
        animInfo = anim.GetCurrentAnimatorStateInfo(0);
        source = GetComponentInChildren<AudioSource>();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (anim.speed != 1) {
            timer++;
        }
        if (timer == 8){
            anim.speed = 1;
            timer = 0;
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("skill01_act")) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.988f) {
            hitNum = 0;
        }
   
	}
    void OnTriggerEnter(Collider co) {
        if (co.gameObject.tag == "Player") {
            player = co.gameObject;
        }

        if (co.gameObject.tag == "Player" && (anim.GetCurrentAnimatorStateInfo(0).IsName("skill01_act")))
        {
            hitNum++;
        }
        if (co.gameObject.tag == "Player" && hitNum == 2)
        {
            source.Play();
            co.gameObject.GetComponent<Animator>().Play("hurt02_1",0,0.2f);
            hitNum = 0;
        }
        else if (co.gameObject.tag == "Player" && (anim.GetCurrentAnimatorStateInfo(0).IsName("attack01_act") || anim.GetCurrentAnimatorStateInfo(0).IsName("skill01_act")))
        {
            co.gameObject.GetComponent<Animator>().Play("hurt01");
            anim.speed = 0f;
            //source.clip(0.1f)
        }
        
    }
}
