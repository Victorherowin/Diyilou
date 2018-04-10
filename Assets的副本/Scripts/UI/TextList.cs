using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entity.Monster;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Player;
/**
	最后的字幕显示控制
*/
public class TextList : MonoBehaviour {
	public Image bgimages;
	public float showTime = 1;
	private float ShowTimeTrigger = 0;
	public float stayTime=170f;
	private float stayTimeTrigger=0;
	public float fadeTime = 1;
	private float fadeTimeTrigger = 0;
	public bool show=true;
	private bool triggeOnce=false;
	public GameObject txtList;
	private float txtHeight=0f;
	private float stepHeight=0f;
	private float height = -2756f;
	public GameObject listObj;
	public GameObject boss;
	public AudioClip audio1;
	public AudioClip audio2;
	private bool startButtleTime = false;
	private float stopTime=1f;
	private float stopTimeTrigger = 0;

	private int temp = 0;
	// Use this for initialization
	void Start () {

		txtList.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,-2756,0);
		foreach (Transform t in txtList.transform) {
			txtHeight += t.gameObject.GetComponent<RectTransform> ().sizeDelta.y;
		}
		Debug.Log (txtHeight);
		stepHeight = (1080+900) / (stayTime/Time.deltaTime);

	}

	// Update is called once per frame
	void Update () {
		if (boss != null)
		if (boss.GetComponentInChildren<BossEnemy> ().HealthPoint <= 0) {
			Time.timeScale = 0.2f;
			startButtleTime = true;
			boss.GetComponentInChildren<AudioSource> ().enabled = false;
		}
		if (startButtleTime)
			stopTimeTrigger += Time.deltaTime;
		if (stopTimeTrigger >= stopTime)
			Time.timeScale = 1f;

		if (show&&triggeOnce==false&&boss==null) {
			GameObject player = GameObject.Find ("Player");
			player.GetComponent<PlayerMove>().enabled = false;
			player.GetComponentInChildren<PlayerEntity>().enabled = false;
			if (!GetComponent<AudioSource> ().isPlaying||(GetComponent<AudioSource> ().clip!=audio1&&GetComponent<AudioSource> ().clip!=audio2)) {
				if (temp == 0 || temp == 1) {
					GetComponent<AudioSource> ().clip = audio1;
					GetComponent<AudioSource> ().Play ();
					temp += 1;
				} else {
					GetComponent<AudioSource> ().clip = audio2;
					GetComponent<AudioSource> ().Play ();
				}
			}

			listObj.SetActive (true);
			if (ShowTimeTrigger < showTime) {
				ShowTimeTrigger += Time.deltaTime;
				bgimages.color = new Color (1, 1, 1, ShowTimeTrigger / showTime);
			} else if (stayTimeTrigger < stayTime) {
				txtList.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,height+=stepHeight,0);
				stayTimeTrigger += Time.deltaTime;
				bgimages.color = new Color (1, 1, 1, 1);
			} else if (fadeTimeTrigger < fadeTime) {
				fadeTimeTrigger += Time.deltaTime;
				bgimages.color = new Color (1, 1, 1, 1 - fadeTimeTrigger / fadeTime);
				if (fadeTimeTrigger >= fadeTime -6)
					SceneManager.LoadScene ("mainMenu");
			} else {
				bgimages.color = new Color (1, 1, 1, 0 );
				show = false;
				triggeOnce = true;

			}

		}
	}

}
