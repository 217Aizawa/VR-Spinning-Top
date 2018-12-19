using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour
{

    public bool isThrown;//投げられたかの判定
    public Vector3 velocity;//速度
    public Vector3 Axis;//軸の向き
    public Vector3 f, old_f,v;   //取得してきた値
    public Quaternion g_rotation,g; //本体とｇの回転


    // Use this for initialization
    void Start()
    {
        ResetSpin();
    }

    // Update is called once per frame
    void Update()
    {


        Axis = old_f;





        f = StringToVector3(UDPReceiver.lastReceivedUDPPacket);

        //f = new Vector3( Input.acceleration.x, -Input.acceleration.z, Input.acceleration.y);
        if ((g * v).magnitude > 1.25&&isThrown!=true) {
            isThrown = true;
            v = f - old_f;
            g_rotation =g;
            velocity = g_rotation*v;
        }


        //        if (isThrown) velocity = f-old_f;
        //else if (f.magnitude <= 1) old_f = f;
        else if (isThrown != true)
        {
            if (f.magnitude < 1.1 && f.magnitude > 0.9) old_f = f;
            g_rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(-old_f.x, old_f.y, -old_f.z));
            v = f - old_f;              //ローカル方向
        }


        Debug.Log(velocity);


        //velocity = new Vector3(0, 0, -5);//変数velocityにVector3構造体をセットする。
        if(isThrown == true)
        {
            ForkParticlePlugin.Instance.Test();
        }
    }

    public void ResetSpin()//初期化
    {
        isThrown = false;
        velocity = Vector3.zero;//zero = (0, 0, 0)
        Axis = Vector3.up;
    }

    public static Vector3 StringToVector3(string input)
    {
        var elements = input.Trim('(', ')').Split(','); // 前後に丸括弧があれば削除し、カンマで分割
        var result = Vector3.zero;
        var elementCount = Mathf.Min(elements.Length, 3); // ループ回数をelementsの数以下かつ3以下にする

        for (var i = 0; i < elementCount; i++)
        {
            float value;

            float.TryParse(elements[i], out value); // 変換に失敗したときに例外が出る方が望ましければ、Parseを使うのがいいでしょう
            result[i] = value;
        }

        return result;
    }


}
