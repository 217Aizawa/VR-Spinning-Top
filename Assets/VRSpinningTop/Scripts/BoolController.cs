﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class BoolController : MonoBehaviour
{
    private float timeCount;

    private void Awake()
    {
        if (GameLoop.isHMD)
        {
            XRSettings.enabled = true;
        }
        else
        {
            XRSettings.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameLoop.isHMD)//PROJECTOR MODE
            timeCount += Time.deltaTime;
        
        if(timeCount >= 3)//3秒経過したら次のシーンに遷移する
        {
            SceneManager.LoadScene(2);
        }
    }
}
