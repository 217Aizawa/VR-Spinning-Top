using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour {
    public float velocity;

    public void ResetString()//初期化
    {
        velocity = 0;
    }
    // Use this for initialization
    void Start () {
        ResetString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void EliminateSlack()//たるみを無くす
    {

    } 
    public void SetResistance()//抵抗
    {

    }
}
