using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeight : MonoBehaviour
{
    public GameObject Text;

    private void Awake()
    {
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
