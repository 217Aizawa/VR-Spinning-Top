using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomaPhysics : MonoBehaviour
{
    //Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //オブジェクトが衝突したとき
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //rb.constraints = RigidbodyConstraints.FreezePosition;//ポジション固定 
        }
    }
}
