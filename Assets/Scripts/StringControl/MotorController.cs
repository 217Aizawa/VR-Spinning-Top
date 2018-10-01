using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

	}

    public void windUpMotor(float speed)    // + で巻取り、- で繰り出し。単位は 1 をモーターフルパワーとする比率
    {

    }

    public void stopMotor() // stop the motor
    {
        windUpMotor(0);
    }

    public void setResistance(float f)  // 提示すべき抵抗。単位は [N]？←要検討
    {

    }
}
