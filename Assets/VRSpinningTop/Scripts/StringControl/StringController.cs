using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour {
    public enum MotorMode { isTrackingHand, isShowingResistance, isRewinding, isFree };
    public GameLoop gameLoop;
    public MotorMode currentMode;       // 現在のモータモード
    public float InitialStringLength;   // 体験開始時の紐繰り出し長さ[mm]
    public float Kp;                    // 比例制御係数
    public float stringDist;
    float targetLength;
    EncoderController enc;
    MotorController motor;

    // Use this for initialization
    void Start () {
        currentMode = MotorMode.isTrackingHand;
        enc = gameObject.GetComponent<EncoderController>();
        motor = gameObject.GetComponent<MotorController>();
    }
	
	// Update is called once per frame
	void Update () {
        stringDist = gameLoop.dist;//糸の巻取り距離
        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                // ここは実は PID 制御でなめらかに目標引き出し長さに移行したい。とりあえず P だけ入れとく。
                float diff = enc.getTotalStringLength() - targetLength;
                // diff が＋　→　目標値のほうが短い　→　巻き取らなければいけない
                motor.windUpMotor(Kp * diff);
                break;

            case MotorMode.isShowingResistance:
                break;

            case MotorMode.isRewinding:
                motor.windUpMotor(1.0f);
                if (enc.getTotalStringLength() < InitialStringLength)
                {
                    motor.stopMotor();
                    currentMode = MotorMode.isFree;
                }
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
    }

    public void setTargetLength(float len)
    // isTrackingHand, isRewinding において、紐の引き出し量の目標値を指定する。
    // 入力：紐引き出し長さ目標値 [mm]
    {
        if (currentMode != MotorMode.isTrackingHand && currentMode != MotorMode.isRewinding)
            return;

        targetLength = len;
    }

    public void setResistance(float resistance)//抵抗    resistance の単位は [N]
    {
        currentMode = MotorMode.isShowingResistance;
        motor.setResistance(resistance);
    }
}
