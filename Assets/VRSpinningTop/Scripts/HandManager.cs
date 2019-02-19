using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class virtualizes Pulling Action of Hands. Uses the detection of joints of Kinect. Might be upgraded to use encoder output.
// 手の「引き」を仮想化するクラス。ひとまず、Kinect の関節検出をベースにした引き速度を返すが、必要に応じてエンコーダ出力の使用を検討する。

public class HandManager : MonoBehaviour
{
    public BodySourceView bodySourceView;
    public SpinController spinController;
    public bool isDebug = true;

    const int MinSampleCount = 10;
    Vector3 lastHandPos;

    List<Vector3> velocitySamples;
    // Start is called before the first frame update
    void Start()
    {
        velocitySamples = new List<Vector3>();
        lastHandPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpinController.isThrown || isDebug)
        {
            Vector3 handPos = bodySourceView.handednessWristPos;
            if (lastHandPos == Vector3.zero)
                lastHandPos = handPos;

            Vector3 v = (handPos - lastHandPos) / Time.deltaTime;
            velocitySamples.Add(v);
            if (velocitySamples.Count > MinSampleCount)
                velocitySamples.RemoveAt(0);

            lastHandPos = handPos;
            
            /*if (isDebug)
                Debug.Log("Hand Velocity " + v);
            */ 
        }
        else
        {
            velocitySamples.Clear();
        }
    }

    // returns Hand Velocity
    public Vector3 handVelocity()
    {
        if (velocitySamples.Count < MinSampleCount)
            return Vector3.zero;

        Vector3 result = Vector3.zero;
        foreach(Vector3 v in velocitySamples)
        {
            result += v;
        }

        result /= velocitySamples.Count;
        return result;
    }
}
