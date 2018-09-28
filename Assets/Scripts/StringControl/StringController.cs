using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour {
    public float velocity;
    public float slackAllowance;

    float localSlack;           // 残りたるみ量
    bool isEliminatingSlack;    // 巻取り中フラグ

    // Use this for initialization
    void Start () {
        ResetString();
	}
	
	// Update is called once per frame
	void Update () {
        // 巻取り中なら
        if (isEliminatingSlack)
        {
            // まだ巻き取り足りないなら
            if (localSlack > slackAllowance)
            {
                // 巻き取る
                gameObject.GetComponent<MotorController>().wind();
            }
            else
            {
                // 巻取りを終了し巻取り中解除
                gameObject.GetComponent<MotorController>().stop();
                isEliminatingSlack = false;
            }
        }
        else // 巻取り中でない＝抵抗感提示中
        {
            Debug.Log(gameObject.GetComponent<EncoderController>().getSpeed());
        }
	}

    public void EliminateSlack(float slackLength)   //弛みをなくす slackLength の単位は [m] 
    {
        isEliminatingSlack = true;
        localSlack = slackLength;
    } 

    public void SetResistance(float resistance)     //抵抗    resistance の単位は [N]
    {
        isEliminatingSlack = false;
        gameObject.GetComponent<MotorController>().setResistance(resistance);
    }

    public void ResetString()
    {
        velocity = 0;
        localSlack = 0;
        isEliminatingSlack = false;
    }
}
