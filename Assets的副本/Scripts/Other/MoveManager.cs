using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveManager : MonoBehaviour {
    private string pre_SceneName;
    public Dictionary<string, Transform> pos = new Dictionary<string, Transform>();
    // Use this for initialization
    void Start () {
        GameObject.DontDestroyOnLoad(gameObject);
        pre_SceneName = SceneManager.GetActiveScene().name;
    }
	
	// Update is called once per frame
	void Update () {
		if (pre_SceneName != SceneManager.GetActiveScene ().name) {
			GameObject[] gates=GameObject.FindGameObjectsWithTag ("MoveGate");
			foreach (GameObject gb in gates) {
				if (gb.GetComponent<MovePoint> ().sceneName == pre_SceneName) {
                    if (pos.ContainsKey(SceneManager.GetActiveScene().name)) {
                        while (GameObject.Find("Player") != null) {
                            GameObject.Find("Player").transform.position = pos[SceneManager.GetActiveScene().name].position;
                            GameObject.Find("Player").transform.rotation = pos[SceneManager.GetActiveScene().name].rotation;
                        }
                        
                    }
                }
			}
			pre_SceneName = SceneManager.GetActiveScene ().name;
		}
	}
}
