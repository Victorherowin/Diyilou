using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
/**
  在初始场景播放公司logo宣传视频
*/
public class CompanyVideoPlay : MonoBehaviour {
    public VideoPlayer player;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    //判断是否播放完成，若播放完，则跳转到下一场景
        if (player.clip.name != "HugElephant Game_1080p.mp4" && (ulong)player.frame >= player.frameCount) {
            SceneManager.LoadScene("mainMenu");
        }
	}
}
