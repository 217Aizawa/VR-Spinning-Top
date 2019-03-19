using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamraHeight : MonoBehaviour
{

    public GameObject cameraParent;
    private void Awake()
    {
        //Camera camera = gameObject.GetComponentInChildren<Camera>();//カメラ取得
        
        if (JudgeController.isHeight == "tall")
        {
            cameraParent.transform.position = new Vector3(0, 1.7f, 0);
        }
        else if(JudgeController.isHeight == "middle")
        {
            cameraParent.transform.position = new Vector3(0, 1.4f, 0);
        }
        else if(JudgeController.isHeight == "short")
        {
            cameraParent.transform.position = new Vector3(0, 1.0f, 0);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
