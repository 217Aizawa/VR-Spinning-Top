using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour {
    public enum MotorMode { isTrackingHand, isShowingResistance, isRewinding, isFree, isCalibrating };
    public GameLoop gameLoop;
    public MotorMode currentMode;       // 現在のモータモード
    public float InitialStringLength;   // 体験開始時の紐繰り出し長さ[mm]
    public float Kp;                    // 比例制御係数

    public float targetLength;          // 目標繰り出し量（参照用に public）
    public float actualLength;          // 実際の繰り出し量（参照用）

    EncoderController enc;
    MotorController motor;
    SerialConnector serialPort;

    // Use this for initialization
    void Start () {
        currentMode = MotorMode.isTrackingHand;
        enc = gameObject.GetComponent<EncoderController>();
        motor = gameObject.GetComponent<MotorController>();
        serialPort = gameObject.GetComponent<SerialConnector>();
        serialPort.Connect(3);
        enc.resetCount(InitialStringLength+1000);
    }
	
	// Update is called once per frame
	void Update () {
        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                // ここは実は PID 制御でなめらかに目標引き出し長さに移行したい。とりあえず P だけ入れとく。
                actualLength = enc.getTotalStringLength();
                float diff = actualLength - targetLength;
                // diff が＋　→　目標値のほうが短い　→　巻き取らなければいけない
                motor.windUpMotor(Kp * diff);
                break;

            case MotorMode.isShowingResistance:
                break;

            case MotorMode.isRewinding:
/*                if (enc.getTotalStringLength() < InitialStringLength)
                {
                    motor.stopMotor();
                    currentMode = MotorMode.isFree;
                } */
                break;

        }
    }

    public void setMotorMode(MotorMode mode)
    // モーターの現在状態を切り替える
    // MotorMode.isTrackingHand = コマ投げ出し前（手の位置に応じて巻き取り・繰り出し）
    // MotorMode.isShowingResistance = コマ投げ出し後
    // MotorMode.isRewinding = プレイ終了後巻取り中
    // MotorMode.isFree = フリー状態
    {
        currentMode = mode;
        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                break;

            case MotorMode.isShowingResistance:
                break;

            case MotorMode.isRewinding:
                motor.windUpMotor(1.0f);
                break;

            case MotorMode.isFree:
                motor.stopMotor();
                break;
        }

    }

    public void setTargetLength(float len)
    // isTrackingHand, isRewinding において、紐の引き出し量の目標値を指定する。
    // 入力：紐引き出し長さ目標値 [m]
    {
        if (currentMode != MotorMode.isTrackingHand && currentMode != MotorMode.isRewinding)
            return;

        targetLength = len * 1000;
    }

    public void setResistance(float resistance)//抵抗    resistance の単位は [N]
    {
        currentMode = MotorMode.isShowingResistance;
        motor.setResistance(resistance);
    }
}
