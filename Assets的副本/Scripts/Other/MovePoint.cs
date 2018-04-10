using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovePoint : MonoBehaviour {
	private bool canMove = false;
	public string sceneName="";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canMove) {
			//if (Input.GetKeyDown (KeyCode.E)) {
				SceneManager.LoadScene(sceneName);
			//}
		}
	}

	void OnTriggerEnter(Collider co){
		canMove = true;
	}
	void OnTriggerExit(Collider co){
		canMove = false;
	}
}
