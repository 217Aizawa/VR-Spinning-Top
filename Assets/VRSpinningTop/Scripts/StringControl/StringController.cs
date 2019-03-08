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
    SerialConnector serialPort;

    [Header("Serial Port")]
    public int portNumber;

    // Use this for initialization
    void Start () {
        currentMode = MotorMode.isTrackingHand;
        enc = gameObject.GetComponent<EncoderController>();
        serialPort = gameObject.GetComponent<SerialConnector>();
        serialPort.Connect(portNumber);
        enc.resetCount(InitialStringLength+1000);
    }

    private void OnDestroy()
    {
        if (!serialPort)
            serialPort.SendChar('A');   // initial FREE MODE
    }

    // Update is called once per frame
    void Update () {
        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                break;

            case MotorMode.isShowingResistance:
                break;

            case MotorMode.isRewinding:
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
        if (serialPort == null)
            return;

        // if mode is unchanged, do nothing. This is to avoid sending the status code to Machine repeatedly.
        if (currentMode == mode)
            return;

        currentMode = mode;

        if (serialPort == null)
            return;

        Debug.Log("Motor Switching to " + currentMode);

        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                serialPort.SendChar('B');
                break;

            case MotorMode.isShowingResistance:
                serialPort.SendChar('C');
                break;

            case MotorMode.isRewinding:
                serialPort.SendChar('E');
                break;

            case MotorMode.isFree:
                serialPort.SendChar('D');
                break;
        }
    }

    public void setTargetLength(float setLengthInMeter)
    // isTrackingHand, isRewinding において、紐の引き出し量の目標値を指定する。
    // 入力：紐引き出し長さ目標値 [m]
    {
        if (currentMode != MotorMode.isTrackingHand && currentMode != MotorMode.isRewinding)
            return;

        targetLength = setLengthInMeter * 1000;
    }


    public void setResistance(float resistance)//抵抗    resistance の単位は [N]
    {
        currentMode = MotorMode.isShowingResistance;
    }

    public void calibrateToLength(float currentLengthInMeter)
    {
        actualLength = currentLengthInMeter * 1000;
        enc.resetCount(actualLength);
    }
}
