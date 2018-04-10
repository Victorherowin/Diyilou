using UnityEngine;

public class TransferSkillTarget : MonoBehaviour
{
    public float DestoryTime = 10.0f;

    // Use this for initialization
    private void Start()
    {
        Invoke("DestorySlef", DestoryTime);
    }

    private void DestorySlef()
    {
        GameObject.Destroy(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}