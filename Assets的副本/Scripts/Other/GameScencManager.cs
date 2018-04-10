using Manager.Save.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScencManager : MonoBehaviour {

    private Scene m_currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Directory.Exists("save"))
            Directory.CreateDirectory("save");
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded+=(s,m)=> m_currentScene = SceneManager.GetActiveScene();
    }

    public bool SaveExist()
    {
        return File.Exists("save/save.json");
    }

    public void SaveGame()
    {
        var json = new SaveConfigJson();
        json.StageName = m_currentScene.name;
        json.Position = Player.PlayerEntity.Instance.transform.position;
        json.Rotation = Player.PlayerEntity.Instance.transform.rotation;

        File.WriteAllText("save/save.json",JsonUtility.ToJson(json,true));
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(0.0f,0.0f,100.0f,100.0f),"重置关卡"))
        {
            SceneManager.LoadSceneAsync(m_currentScene.name);
        }
    }
}
