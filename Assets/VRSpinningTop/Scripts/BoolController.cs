using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class BoolController : MonoBehaviour
{
    private float timeCount;

    private void Awake()
    {
        if (JudgeController.isJudge)
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
        if (!JudgeController.isJudge)//PROJECTOR MODE
            timeCount += Time.deltaTime;
        
        if(timeCount >= 4)//4秒経過したら次のシーンに遷移する
        {
            SceneManager.LoadScene(2);
        }
    }
}
