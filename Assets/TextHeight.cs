using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeight : MonoBehaviour
{
    public GameObject Text;

    private void Awake()
    {
        /*
        if (JudgeController.isHeight == "tall")
        {
            Text.transform.position = new Vector3(0, 3.0f, 4);
        }
        else if (JudgeController.isHeight == "middle")
        {
            Text.transform.position = new Vector3(0, 2.5f, 4);
        }
        else if (JudgeController.isHeight == "short")
        {
            Text.transform.position = new Vector3(0, 2.05f, 4);
        }
        */
        if (JudgeController.isJudge == true && JudgeController.isHeight != null)
        {
            Text.transform.position = new Vector3(0, 3.2f, 4);
        }
        else if (JudgeController.isHeight == "tall")
        {
            Text.transform.position = new Vector3(0, 3.0f, 4);
        }
        else if (JudgeController.isHeight == "middle")
        {
            Text.transform.position = new Vector3(0, 2.5f, 4);
        }
        else if (JudgeController.isHeight == "short")
        {
            Text.transform.position = new Vector3(0, 2.05f, 4);
        }
    }
}
