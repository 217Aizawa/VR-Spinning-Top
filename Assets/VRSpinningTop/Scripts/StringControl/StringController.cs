using System.Collections.Generic;
using UnityEngine;

struct SpeedRecord
{
    public float timestamp;
    public float speed;
};

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

    [Header("Pulling Status")]
    public bool isPulling;
    public float timeStartup;      // ひきはじめまでの時間（最高引き速度の５０％に到達する時間）
    public float timeTotal;        // 総引き時間
    public float maxPullingSpeed;  // 最高引き速度

    float timeZero;
    List<SpeedRecord> records;

    // Use this for initialization
    void Start () {
        currentMode = MotorMode.isTrackingHand;
        enc = gameObject.GetComponent<EncoderController>();
        serialPort = gameObject.GetComponent<SerialConnector>();
        serialPort.Connect(portNumber);
        enc.resetCount(InitialStringLength+1000);
        records = new List<SpeedRecord>();
        isPulling = false;
    }

    private void OnDestroy()
    {
        if (!serialPort)
        {
            serialPort.SendChar('A');   // initial FREE MODE
        }
    }

    // Update is called once per frame
    void Update () {
        switch (currentMode)
        {
            case MotorMode.isTrackingHand:
                break;

            case MotorMode.isShowingResistance:
                
                SpeedRecord sr; // = new SpeedRecord();
                sr.speed = -enc.getSpeed();                 // pull is negative, but we want positive value here
                sr.timestamp = Time.fixedTime - timeZero;
                records.Add(sr);

                float pulledStringLength = -enc.getTotalStringLength();

//                Debug.Log("Pull Speed " + sr + " Length " + pulledStringLength);

                if( isPulling && pulledStringLength > 1.0f )
                {
                    isPulling = false;
                    maxPullingSpeed = 0;
                    foreach(SpeedRecord r in records)
                    {
                        if( maxPullingSpeed < r.speed )
                        {
                            maxPullingSpeed = r.speed;
                        }
                    }

                    foreach (SpeedRecord r in records)
                    {
                        if (r.speed > maxPullingSpeed * 0.5f)
                        {
                            timeStartup = r.timestamp;
                            break;
                        }
                    }

                    timeTotal = records[records.Count - 1].timestamp;

                    Debug.Log(timeStartup + " | " + timeTotal + " | " + maxPullingSpeed);
                }
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
                serialPort.SendChar('B');       // 'C' でブレーキの予定だったか、重すぎるのでフリーに
                timeZero = Time.fixedTime;
                records.Clear();
                calibrateToLength(0);
                isPulling = true;
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

    public float getSpeed()
    {
        return enc.getSpeed();
    }

}
