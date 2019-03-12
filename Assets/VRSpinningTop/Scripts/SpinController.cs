using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    public static bool isThrown;//投げられたかの判定
    public Vector3 velocity;//速度
    public float rotationSpeedZ = 0.0f;
    public Vector3 Axis;//軸の向き
    public Vector3 f, old_f, v;   //取得してきた値

    [Header("New Koma Device")]
    public bool useNewDevice;
    public KomaDeviceController komaDeviceController;
    public int KomaPort;

    [Space(10)]

    //vはローカルな方向
    //fは加速度方向
    //old_fは安定している重力方向
    public Quaternion g_rotation,g; //本体とｇの回転

    // 投げ出し判定加速度
    public float ThrowOffThreshold = 1.25f;//Inspector上の数値が優先される

    // Use this for initialization
    void Start()
    {
        ResetSpin();
        komaDeviceController.Connect(KomaPort);
    }

    // Update is called once per frame
    void Update()
    {
        Axis = old_f;

        if (useNewDevice)
            f = komaDeviceController.getAcceleration();
        else
            f = StringToVector3(UDPReceiver.lastReceivedUDPPacket);
        

        //f = new Vector3( Input.acceleration.x, -Input.acceleration.z, Input.acceleration.y);
        if ((g * v).magnitude > ThrowOffThreshold && isThrown!=true)    // 投げ出し判定
        {
            isThrown = true;
            v = f - old_f;
            g_rotation = g;
            velocity = g_rotation*v;//追加

            velocity.x = velocity.x * -1;//velocityのX・Z軸の正負が反転しているのでここで正常に戻す。
            //velocity.z = velocity.z * -1;//確認済み
            StartCoroutine("CalculateRotationZ");
        }

        //if (isThrown) velocity = f-old_f;
        //else if (f.magnitude <= 1) old_f = f;
        else if (isThrown != true)  // 投げ出し判定前（重力方向測定）
        {
            if (f.magnitude < 1.1 && f.magnitude > 0.9) old_f = f;
            g_rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(old_f.x, old_f.y, old_f.z));//-old_f.x, old_f.y, -old_f.z)
            v = f - old_f;              //ローカル方向
        }

//        Debug.Log("koma vel." + velocity);

        //velocity = new Vector3(0, 0, -5);//変数velocityにVector3構造体をセットする。
        if(isThrown == true)
        {
        }
    }

    IEnumerator CalculateRotationZ()
    {
        Vector3 acc;
        while (true)
        {
            if (useNewDevice)
                acc = komaDeviceController.getAcceleration();
            else
                acc = StringToVector3(UDPReceiver.lastReceivedUDPPacket);

            if (acc.magnitude < 0.2f)  // almost free in the air
                break;

            yield return new WaitForSecondsRealtime(0.01f);
        }

        // if Z direction is changing, the acceleration will be observed on Z
        rotationSpeedZ = f.z;
        Debug.Log("Observed Z rotation" + rotationSpeedZ);

        yield break;
    }

    public void SetSuccessEffect(float delay = 0.0f)
    {
        Invoke("StartSuccessEffect", delay);
    }

    void StartSuccessEffect()
    {
        ForkParticlePlugin.Instance.Test();
    }

    public void ResetSpin()//初期化
    {
        isThrown = false;
        velocity = Vector3.zero;//zero = (0, 0, 0)
        Axis = Vector3.up;
    }

    private static Vector3 StringToVector3(string input)
    {
        var elements = input.Trim('(', ')').Split(','); // 前後に丸括弧があれば削除し、カンマで分割
        var elementCount = Mathf.Min(elements.Length, 3); // ループ回数をelementsの数以下かつ3以下にする
        var result = Vector3.zero;

        /*var elements = input.Trim('(', ')').Split(','); // 前後に丸括弧があれば削除し、カンマで分割
        var result = Vector3.zero;
        var elementCount = Mathf.Min(elements.Length, 3); // ループ回数をelementsの数以下かつ3以下にする
        */
        for (var i = 0; i < elementCount; i++)
        {
            float value;

            float.TryParse(elements[i], out value); // 変換に失敗したときに例外が出る方が望ましければ、Parseを使うのがいいでしょう
            result[i] = value;
        }

        return result;
    }
}
