using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TestTalk : MonoBehaviour {
	private GameObject gb;
	private GameObject canvas;
    public List<string> nameIn = new List<string>();
    public List<string> talkIn = new List<string>();
	public List<AudioClip> audioIn = new List<AudioClip> ();
    public bool talkOnce = true;
	public bool PlayOnAwake = false;
	public bool stopWhenPlaying=true;
	public bool autoPlay = false;
	public bool poseStop=false;
	public float timePart=3f;
	public bool playVideo = false;
	public GameObject skyCamera;

	private List<int> readyAudio=new List<int>();
    private Text nameArea=null;
    private Text talkArea = null;
	private bool isThisObj = false;
	private AudioSource playerTalk;
	private float preTime = 0f;
	private bool resetThis=false;
	private bool playOver = false;
	private bool videoReset=false;
	private bool startPlay = false;
	private List<GameObject> monsterList=new List<GameObject>();

    private int i = 0;
	// Use this for initialization
	void Start () {
		
        canvas = GameObject.Find("Canvas");
		playerTalk = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<AudioSource> ();
        foreach (Transform child in canvas.transform) {
            if (child.gameObject.name == "TalkFme") {
                gb = child.gameObject;
            }
        }
        foreach (Transform child in gb.transform)
        {
            if (child.gameObject.name == "name")
            {
                nameArea = child.gameObject.GetComponent<Text>();
            }
            if (child.gameObject.name == "content")
            {
                talkArea = child.gameObject.GetComponent<Text>();
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		if (playVideo&&videoReset==false&&(canvas.GetComponent<PlayUi>().isTalking == true||PlayOnAwake)) {
			skyCamera.SetActive (false);
			GetComponent<VideoPlayer> ().Play ();
			//Time.timeScale = 0f;
			videoReset = true;
			canvas.SetActive (false);
			player.GetComponent<PlayerMove>().enabled = false;
			player.GetComponentInChildren<PlayerEntity>().enabled = false;
			startPlay = true;
		}
		if (playVideo==true && playOver == false&&startPlay==true) {
			
			if (!GetComponent<VideoPlayer> ().isPlaying)//也可以这么判断是否播放完毕  
			{  
				skyCamera.SetActive (true);
				player.GetComponent<PlayerMove>().enabled = true;
				player.GetComponentInChildren<PlayerEntity>().enabled = true;
				playOver = true;
				GetComponent<VideoPlayer> ().enabled = false;
				//Time.timeScale = 1.0f;
				startPlay = false;
				canvas.SetActive (true);
			}  
		}
		else if (((canvas.GetComponent<PlayUi>().isTalking == true&&isThisObj)||PlayOnAwake)&&nameIn.Count!=0) {
			if (i == 0&&resetThis==false) {
				preTime =Time.realtimeSinceStartup;
				resetThis = true;
			}
            
			if (stopWhenPlaying) {
				player.GetComponent<PlayerMove>().enabled = false;
				if (poseStop) {
					GameObject []gbs=GameObject.FindGameObjectsWithTag ("Monster");
					foreach (GameObject g in gbs) {
						monsterList.Add (g);
					}
					foreach (GameObject g in monsterList) {
						g.SetActive (false);
					}
				}else
					Time.timeScale = 0f;
				player.GetComponentInChildren<PlayerEntity>().enabled = false;
			}

            gb.SetActive(true);
            if (i < nameIn.Count)
            {
                nameArea.text = nameIn[i];
                talkArea.text = talkIn[i];

				if ((!playerTalk.isPlaying || playerTalk.clip != audioIn [i])&&i<audioIn.Count) {
					if (!readyAudio.Contains (i)) {
						playerTalk.clip = audioIn [i];
						playerTalk.Play ();
						readyAudio.Add (i);
					}
				}
					
            }
			if (autoPlay) {
				if (Time.realtimeSinceStartup - preTime >= timePart&&!playerTalk.isPlaying) {
					i++;
					preTime =Time.realtimeSinceStartup;
					if (i >= nameIn.Count) {
						canvas.GetComponent<PlayUi>().isTalking = false;
						gb.SetActive(false);
						i = 0;
						if (stopWhenPlaying) {
							if (poseStop) {
								foreach (GameObject g in monsterList) {
									g.SetActive (true);
								}
							} else {
								Time.timeScale = 1f;
							}
							player.GetComponent<PlayerMove>().enabled = true;
							player.GetComponentInChildren<PlayerEntity>().enabled = true;
						}

						readyAudio.Clear ();
						resetThis = false;

						if (talkOnce) {
							Destroy(this);
						}
						videoReset = false;
						playOver = false;
						GetComponent<VideoPlayer> ().enabled = true;
					}
				}
			}
			if ((Input.GetMouseButtonDown(0)&&autoPlay==false)) {
                i++;
                if (i >= nameIn.Count) {
                    canvas.GetComponent<PlayUi>().isTalking = false;
                    gb.SetActive(false);
                    i = 0;
					if (stopWhenPlaying) {
						if (poseStop) {
							foreach (GameObject g in monsterList) {
								g.SetActive (true);
							}
						} else {
							Time.timeScale = 1f;
						}
						player.GetComponent<PlayerMove>().enabled = true;
						player.GetComponentInChildren<PlayerEntity>().enabled = true;
					}

					readyAudio.Clear ();
                    if (talkOnce) {
						Destroy(gameObject);
                    }
                }
            }
        }
       
	}

	void OnTriggerEnter(Collider co){
		if (co.gameObject.name == "Player") {
			canvas.GetComponent<PlayUi> ().isTalking = true;
			isThisObj = true;
		}
	}

    void OnDrawGizmos() {
        if (talkIn.Count > nameIn.Count)
        {
            talkIn.RemoveAt(talkIn.Count - 1);
        }
        else if (talkIn.Count < nameIn.Count)
        {
            talkIn.Add("null");
        }
		if (audioIn.Count > nameIn.Count)
		{
			audioIn.RemoveAt(audioIn.Count - 1);
		}
		else if (audioIn.Count < nameIn.Count)
		{
			audioIn.Add(null);
		}
    }
}
