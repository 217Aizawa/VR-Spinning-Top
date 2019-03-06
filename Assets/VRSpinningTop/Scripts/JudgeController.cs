using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JudgeController : MonoBehaviour
{
    public static bool isJudge;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
