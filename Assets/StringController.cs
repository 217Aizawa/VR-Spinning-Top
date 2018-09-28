using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour {
    public float velocity;

    public void ResetString()
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
    public void EliminateSlack()//弛みをなくす
    {

    } 
    public void SetResistance()//抵抗
    {

    }
}
