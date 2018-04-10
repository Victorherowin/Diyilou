using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
  游戏主菜单的选择控制脚本，通过外部UI绑定对其进行调用
*/
public class MenuChoose : MonoBehaviour {
    private GameObject gb;
    private void Start()
    {
      //不显示加载图
        gb = GameObject.Find("LoadingImg");
        gb.SetActive(false);
    }

    public void beginPlay() {
      //开启异步加载场景
        StartCoroutine(LoadYourAsyncScene());

    }
    public void endPlay() {
        Application.Quit();
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("q01a");

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
          //场景未加载成功之前显示加载图片
            gb.SetActive(true);
            yield return null;
        }
    }
    //控制bgm播放
    public void changeBgm() {
        if (GameObject.Find("BgmController").GetComponent<CloasBgm>().bgmClosed)
            GameObject.Find("BgmController").GetComponent<CloasBgm>().bgmClosed = false;
        else
            GameObject.Find("BgmController").GetComponent<CloasBgm>().bgmClosed = true;
        Debug.Log("change");
    }
}
