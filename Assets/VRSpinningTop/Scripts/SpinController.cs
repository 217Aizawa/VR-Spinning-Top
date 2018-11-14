using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour
{

    public bool isThrown;//投げられたかの判定
    public Vector3 velocity;//速度
    public Vector3 Axis;//軸の向き
    Vector3 f, old_f;   //取得してきた値


    // Use this for initialization
    void Start()
    {
        ResetSpin();
    }

    // Update is called once per frame
    void Update()
    {



        Axis = Input.acceleration;





        f = StringToVector3(UDPReceiver.lastReceivedUDPPacket);

        if (f.magnitude > 1.5) { isThrown = true; velocity = f - old_f; }

        //        if (isThrown) velocity = f-old_f;
        else if (f.magnitude <= 1) old_f = f;

        Debug.Log(f.magnitude);


        //velocity = new Vector3(0, 0, -5);//変数velocityにVector3構造体をセットする。

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
