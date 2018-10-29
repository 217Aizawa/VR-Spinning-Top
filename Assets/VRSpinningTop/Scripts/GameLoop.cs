using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {

    public SpinController spinController;//型名 変数名 (SpinController s)。gameObjectのSpinControllerとは違う
    public StringController stringController;//世界の中にあるgameObjectをここに入れる。
    public KinectController kinectController;//そうすることで、spinControllerの変数を使用することができる。
    public GameObject koma;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (spinController.isThrown)//スペースキーが押されたら。
        {
            koma.GetComponent<Rigidbody>().velocity = spinController.velocity;//スピンコントローラの速度(z方向に速度5)を、コマに代入
        }
	}
}
