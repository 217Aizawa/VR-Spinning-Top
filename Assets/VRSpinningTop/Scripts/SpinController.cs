using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    public static bool isThrown;//投げられたかの判定
    public Vector3 velocity;//速度
    public float rotationSpeedZ = 0.0f;
    public float angleZ;
    public Vector3 Axis;//軸の向き
    public Vector3 currentForce, oldForce, v;   //取得してきた値

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
    public float ThrowOffThreshold = 0.2f;//Inspector上の数値が優先される

    // Use this for initialization
    void Start()
    {
        ResetSpin();
        komaDeviceController.Connect(KomaPort);
    }

    // Update is called once per frame
    void Update()
    {
        Axis = oldForce;

        if (useNewDevice)
            currentForce = komaDeviceController.getAcceleration();
        else
            currentForce = StringToVector3(UDPReceiver.lastReceivedUDPPacket);
        

        //currentForce = new Vector3( Input.acceleration.x, -Input.acceleration.z, Input.acceleration.y);
        if (currentForce.magnitude < ThrowOffThreshold && isThrown != true)    // 投げ出し判定
        {
            isThrown = true;
            velocity = g_rotation * v;      // 最後に加えられていた力を打ち出し速度とする
            velocity.x = velocity.x * -1;   //velocityのX軸の正負が反転しているのでここで正常に戻す。

            // ｚ軸の向きがぐるぐる回っていれば、ｚ軸の加速度（遠心力）として観測される
            rotationSpeedZ = currentForce.z;
            Debug.Log("Observed Z rotation: " + rotationSpeedZ);

            // 投げ出し時のｚ軸の傾き（度）
            angleZ = Vector3.Angle(g_rotation * new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            Debug.Log("Oberved Z slant: " + angleZ);
        }

        //if (isThrown) velocity = currentForce-old_f;
        //else if (currentForce.magnitude <= 1) old_f = currentForce;
        else if (isThrown != true)  // 投げ出し判定前（重力方向測定）
        {
            if (0.9f < currentForce.magnitude && currentForce.magnitude < 1.1f)
                oldForce = currentForce;
            g_rotation = Quaternion.FromToRotation(new Vector3(0,0,1), new Vector3(oldForce.x, oldForce.y, oldForce.z));//-old_f.x, old_f.y, -old_f.z)
            v = currentForce - oldForce;              //　加えられている力（ローカル方向）
        }

//        Debug.Log("koma vel." + velocity);

        //velocity = new Vector3(0, 0, -5);//変数velocityにVector3構造体をセットする。
        if(isThrown == true)
        {
        }
    }

    public void SetSuccessEffect(float delay = 0.0f)
    {
        Invoke("StartSuccessEffect", delay);
    }

    void StartSuccessEffect()
    {
        ForkParticlePlugin.Instance.Test();
    }

    public void StopSuccessEffect()
    {
        ForkParticlePlugin.Instance.InvalidateObjects();
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
