using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JudgeController : MonoBehaviour
{
    public static bool isJudge;
    public static string isHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HeightSetting();
        if (Input.GetKey(KeyCode.T))
        {
            isJudge = true;//static bool
            SceneManager.LoadScene(1);
        }else if (Input.GetKey(KeyCode.F))
        {
            isJudge = false;
            SceneManager.LoadScene(1);
        }
    }

    public void HeightSetting()
    {
        if (Input.GetKey(KeyCode.Keypad1))//テンキー1
        {
            isHeight = "tall";
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            isHeight = "middle";
        }
        else if (Input.GetKey(KeyCode.Keypad3))
        {
            isHeight = "short";
            Debug.Log(isHeight);
        }
    }
}
