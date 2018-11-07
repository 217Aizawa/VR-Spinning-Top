﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour {

    public SpinController spinController;//型名 変数名 (SpinController s)。gameObjectのSpinControllerとは違う
    public StringController stringController;//世界の中にあるgameObjectをここに入れる。
    public KinectController kinectController;//そうすることで、spinControllerの変数を使用することができる。
    public GameObject koma;

    private float afterTime = 0;//投げ終わってからの時間

    public GameObject Great;
    public GameObject AdviseMoreFast;
    public GameObject AdviseMoreSlow;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        TimeCounter();
        
        if (spinController.isThrown)//スペースキーが押されたら。
        {
            koma.GetComponent<Rigidbody>().velocity = spinController.velocity;//スピンコントローラの速度(z方向に速度5)を、コマに代入
        }

        //コマの速度が10以上20以下かつ、投げ終わってから3秒以上経過した場合
        if(10 <= koma.GetComponent<Rigidbody>().velocity.z  && koma.GetComponent<Rigidbody>().velocity.z <= 20 && 3 <= afterTime)
            //最終的には、リザルト画面で表示させる。
        {
            Debug.Log("もう少しゆっくり投げてください！！");
            AdviseMoreSlow.SetActive(true);//アドバイステキストをアクティブにする
        }
        //コマの速度が1以上10以下かつ、投げ終わってから3秒以上経過した場合
        else if (1 <= koma.GetComponent<Rigidbody>().velocity.z && koma.GetComponent<Rigidbody>().velocity.z <= 10 && 3 <= afterTime)
        {
            Debug.Log("もう少し速く投げてください！！");
            AdviseMoreFast.SetActive(true);//アドバイステキストをアクティブにする
        }
        else if (3 <= afterTime)
        {
            Debug.Log("素晴らしい");
            Great.SetActive(true);
        }
    }

    void TimeCounter()//コマを投げ終わってからの時間を計る。
    {
        if (spinController.isThrown)//スペースキーが押されたら。
        {
            afterTime += Time.deltaTime;
        }
    }
}
