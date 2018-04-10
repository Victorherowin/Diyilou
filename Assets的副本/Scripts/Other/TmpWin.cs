using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpWin : MonoBehaviour {
    public GameObject WinWindow;

    private void OnTriggerEnter(Collider other)
    {
        WinWindow.SetActive(true);
        InputManager.Enable = false;
    }
}
