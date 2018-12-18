using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour {

    SerialConnector serialPort;

    // Use this for initialization
    void Start () {
        serialPort = gameObject.GetComponent<SerialConnector>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
            windUpMotor(1.0f);
        if (Input.GetKey(KeyCode.Alpha0))
            windUpMotor(0);
        if (Input.GetKey(KeyCode.Minus))
            windUpMotor(-1.0f);
        if (Input.GetKey(KeyCode.X))
            setResistance(1.0f);


    }

    public void windUpMotor(float speed)    // + で巻取り、- で繰り出し。単位は 1 をモーターフルパワーとする比率
    {
        Debug.Log("Motor Speed" + speed);
        if (speed > 0)
            serialPort.SendChar('+');
        else if (speed < 0)
            serialPort.SendChar('-');
        else
            serialPort.SendChar('0');
    }

    public void stopMotor() // stop the motor
    {
        windUpMotor(0);
    }

    public void setResistance(float f)  // 提示すべき抵抗。単位は [N]？←要検討
    {
        if (f > 0)
            serialPort.SendChar('=');
        else
            serialPort.SendChar('0');
    }
}
