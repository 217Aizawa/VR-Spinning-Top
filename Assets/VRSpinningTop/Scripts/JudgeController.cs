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
        GameLoop.isHMD = false;
    }

    // Update is called once per frame
    void Update()
    {
        //HeightSetting();


        if (Input.GetKey(KeyCode.V))
        {
            GameLoop.isHMD = true;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            GameLoop.isHMD = false;
            SceneManager.LoadScene(2);
            
        }

        if (GameLoop.isHMD)
        {
            if (Input.GetKey(KeyCode.Alpha1))//テンキー1
            {
                isHeight = "tall";
                SceneManager.LoadScene(1);
                Debug.Log("tall");
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                isHeight = "middle";
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                isHeight = "short";
                SceneManager.LoadScene(1);
            }
        }
    }

    public void HeightSetting()
    {

    }
}
