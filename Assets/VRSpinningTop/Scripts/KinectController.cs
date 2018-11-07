using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class KinectController : MonoBehaviour {

    public BodySourceView BodySourceView;

    public Vector3 wristPosition;//手首の位置
    public Vector3 spinPosition;//コマの位置
    private int handedness;//プライベート変数　1= 右　-1=左　0=検出失敗

    //あとで、BodySourceViewからhandednessを持ってくる

    // Use this for initialization
    void Start () {
        /*wristPosition = gameObject.transform.position;
        spinPosition = wristPosition;*/
	}
	
	// Update is called once per frame
	void Update () {

        if(BodySourceView.handedness == -1) {
            wristPosition = BodySourceView.handednessWristPos;
            spinPosition = wristPosition;
            Debug.Log("Detected LeftWrist");

        } else if(BodySourceView.handedness == 1)
        {
            wristPosition = BodySourceView.handednessWristPos;
            spinPosition = wristPosition;
            Debug.Log("Detected RightWrist");
        }
    }

    /*public int DetectHandedness()//利き腕判定　戻り値 1=右, -1=左, 0=検出失敗
    {
        return 1;
    }*/
}
