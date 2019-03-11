﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public static Camera SubCamera;
    public GameObject mainCamera;
    public static float cntTime;
    Vector3 cmOffset;
    public Vector3 Position;
    public GameLoop gl;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        ChangeCamera();
        FindKomaTag();
        Position = transform.position;
    }

    void FindKomaTag()//本番用
    {
        Debug.Log("Camera");
        if (GameObject.FindGameObjectsWithTag("KomaChild").Length == 1)
        {
            GameObject target;
            target = GameObject.FindWithTag("KomaChild");

            cmOffset = mainCamera.transform.position - target.transform.position;
            Debug.Log("cmOffset" + cmOffset);

            cmOffset.Normalize();
            cmOffset = cmOffset * 1f;//0.2f
            cmOffset.y = 0.1f;

            Vector3 arrow = target.transform.position + cmOffset - mainCamera.transform.position;
            transform.parent.transform.position = arrow;


            transform.LookAt(target.transform);

            Debug.Log("sub camera at " + transform.position);
        }
    }

    void ChangeCamera()//カメラ切り替え関数
    {
        if (gl.gameState == GameLoop.GameState.result)//リザルト表示なら
        {
            gameObject.GetComponent<Camera>().depth = 10;//カメラの深度をプラスする

        }
    }
}