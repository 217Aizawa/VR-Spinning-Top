using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamraHeight : MonoBehaviour
{
    public GameLoop gameLoop;
    public GameObject cameraParent;
    private void Awake()
    {
        //Camera camera = gameObject.GetComponentInChildren<Camera>();//カメラ取得
    }
    // Start is called before the first frame update
    void Start()
    {
        if (GameLoop.isHMD)
        {
            if (JudgeController.isHeight == "tall")
            {
                cameraParent.transform.position = new Vector3(0, 1.7f, 0f);
            }
            else if (JudgeController.isHeight == "middle")
            {
                cameraParent.transform.position = new Vector3(0, 1.4f, 0);
            }
            else if (JudgeController.isHeight == "short")
            {
                cameraParent.transform.position = new Vector3(0, 1.0f, 0);
            }
        }
        else
            cameraParent.transform.position = new Vector3(0, 0.8f, -0.2f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
