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
        isJudge = false;
    }

    // Update is called once per frame
    void Update()
    {
        //HeightSetting();


        if (Input.GetKey(KeyCode.V))
        {
            isJudge = true;//static bool
            Debug.Log("true");
        }
        else if (Input.GetKey(KeyCode.P))
        {
            isJudge = false;
            SceneManager.LoadScene(1);
        }

        if(isJudge)
        {

            if (Input.GetKey(KeyCode.Keypad1))//テンキー1
            {
                isHeight = "tall";
                SceneManager.LoadScene(1);
                Debug.Log("tall");
            }
            else if (Input.GetKey(KeyCode.Keypad2))
            {
                isHeight = "middle";
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKey(KeyCode.Keypad3))
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
