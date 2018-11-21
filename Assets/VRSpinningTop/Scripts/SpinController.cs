using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour {

    public bool isThrown;//投げられたかの判定
    public Vector3 velocity;//速度
    public Vector3 Axis;//軸の向き

	// Use this for initialization
	void Start () {
        ResetSpin();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            velocity = new Vector3(0, 0, 15);//変数velocityにVector3構造体をセットする。
            isThrown = true;
        }

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
}
