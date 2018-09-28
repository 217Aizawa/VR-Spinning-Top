using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectController : MonoBehaviour {

    public Vector3 wristPosition;
    public Vector3 spinPosition;
    private int handedness;//1= 右　-1=左　0=検出失敗
	// Use this for initialization
	void Start () {
        wristPosition = gameObject.transform.position;
        spinPosition = wristPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public int DetectHandedness()//利き腕判定　戻り値 1=右, -1=左, 0=検出失敗
    {
        return 1;
    }
}
