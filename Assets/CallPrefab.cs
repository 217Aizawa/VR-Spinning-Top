using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPrefab : MonoBehaviour {

    public GameObject Koma;

    GameObject Obj;//入れ物
    GameObject Parent;
	// Use this for initialization
	void Start ()
    {
		Parent = GameObject.Find("Parent");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            Obj = (GameObject)Instantiate(Koma, this.transform.position, Quaternion.identity, Parent.transform);  //両手上げでプレファブ削除？
            //Obj.transform.parent = Parent.transform;
        }
	}
}
