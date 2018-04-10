using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
  用于判断是否设置了关闭bgm，若设置了，则关闭捆绑在MainCamera上的AudioSource
*/
public class CloasBgm : MonoBehaviour {
    public bool bgmClosed = false;
    // Use this for initialization
    // Update is called once per frame
    private void Start()
    {
      //在切换场景时不销毁
        DontDestroyOnLoad(this.gameObject);
    }
    void Update () {
        if(bgmClosed)
            Camera.main.GetComponent<AudioSource>().enabled = false;
        else
            Camera.main.GetComponent<AudioSource>().enabled = true;
		Debug.Log (bgmClosed);
    }
    //通过外部按钮事件对其进行调用，改变bgm播放属性
    public void changeStage() {
        if (bgmClosed)
            bgmClosed = false;
        else
            bgmClosed = true;
    }
}
