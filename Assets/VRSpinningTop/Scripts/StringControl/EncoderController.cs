using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct EncoderCount
{
    public int count;
    public float dt;
}

public class EncoderController : MonoBehaviour {
    public float ItomakiRadius; // 糸巻き半径 [mm]
    public int EncoderCounts;   // 一周のエンコーダカウント数

    List<EncoderCount> samples;
    int formerCount;
    public int currentCount;
    
    // 定数
    float AnglePerCount;    // １カウントあたりの角度 [rad]
    float LengthPerCount;   // １カウントあたりの繰り出し量 [mm]

    SerialConnector serialPort;
    
	// Use this for initialization
	void Start () {
        samples = new List<EncoderCount>();

        AnglePerCount = 2.0f * Mathf.PI / 360 / EncoderCounts;
        LengthPerCount = AnglePerCount * ItomakiRadius;

        serialPort = gameObject.GetComponent<SerialConnector>();
	}
	
	// Update is called once per frame
	void Update () {
        currentCount = serialPort.GetRotationCount();

        EncoderCount ec;
        ec.dt = Time.deltaTime;
        ec.count = (currentCount - formerCount);
        samples.Add(ec);
        if( samples.Count > 4)
        {
            samples.RemoveAt(0);
        }

        formerCount = currentCount;
	}

    public float getSpeed()
    {
        EncoderCount sum;
        sum.count = 0;
        sum.dt = 0;

        foreach(EncoderCount item in samples)
        {
            sum.count += item.count;
            sum.dt += item.dt;
            if (sum.count == 4)
                break;
        }
        return sum.count / sum.dt;
    }

    public float getTotalAngle()
    {
        return currentCount * AnglePerCount;
    }

    public float getTotalStringLength()
    {
        return currentCount * LengthPerCount;
    }

    public void resetCount(float extractedLength = 0)
    // 引き出された糸の長さを引数として、エンコーダのカウントをリセットする。
    // 完全に巻き取った状態で呼ぶときは、引数無しでよい。
    {
        currentCount = (int)(extractedLength / LengthPerCount);
//        Debug.Log("Encoder reset to " + extractedLength + "/" + LengthPerCount + "/" + currentCount);
        formerCount = currentCount;
        if( serialPort )
            serialPort.SetRotationCount(currentCount);
    }
}
