using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
/**
	游戏界面的UI
*/
public class PlayUi : MonoBehaviour {
	private GameObject stopUi;	//暂停后显示的UI
	private GameObject talkFrame;	//对话框
    private GameObject bgmBt;	//关闭bgm的按键
	private GameObject bleedMax,bleedNow;	//血量条
	private bool isStop = false;	//是否正在暂停的flag
	public bool isTalking = false;	//是否正在谈话的flag
	// Use this for initialization
	void Start () {
		foreach (Transform child in gameObject.transform) {
			if (child.gameObject.name == "stopPanel") {
				stopUi = child.gameObject;
                foreach (Transform cc in child) {
                    if (cc.gameObject.name == "Toggle") {
                        bgmBt = cc.gameObject;
                    }
                }
			}
			if (child.gameObject.name == "bleed_bg") {
				bleedMax = child.gameObject;
				foreach (Transform cc in child) {
					if (cc.gameObject.name == "bleed") {
						bleedNow = cc.gameObject;
					}
				}
			}
			if (child.gameObject.name == "TalkFme") {
				talkFrame = child.gameObject;
			}
		}
        bgmBt.GetComponent<Toggle>().isOn = GameObject.Find("BgmController").GetComponent<CloasBgm>().bgmClosed;
    }

	// Update is called once per frame
	void Update () {
		float bleedwidth = GameObject.Find ("Player").GetComponentInChildren<PlayerEntity> ().HealthPoint*1.0f / GameObject.Find ("Player").GetComponentInChildren<PlayerEntity> ().MaxHealthPoint * 261.0f;
		bleedNow.GetComponent<RectTransform> ().sizeDelta = new Vector2 (bleedwidth, 9);

		if (isTalking) {

		}else if (Input.GetKeyDown (KeyCode.Escape)) {
			if (isStop) {
				isStop = false;
				Time.timeScale = 1;
				stopUi.SetActive (false);
			} else {
				isStop = true;
				Time.timeScale = 0;
				stopUi.SetActive (true);
			}

		}
	}

}
