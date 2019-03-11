using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public static Camera SubCamera;
    public GameObject target;
    public static float cntTime;
    Vector3 cmOffset;
    public Vector3 Position;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChangeCamera();
        FindKomaTag();
        //DebugFindKomaTag();
        Position = transform.position;
    }

    void FindKomaTag()//本番用
    {
        if (GameObject.FindGameObjectsWithTag("KomaChild").Length == 1)
        {
            target = GameObject.FindWithTag("KomaChild");

            cmOffset = transform.position - target.transform.position;
            Debug.Log("cmOffset" + cmOffset);

            cmOffset.Normalize();
            cmOffset = cmOffset * 0.2f;
            cmOffset.y = 0.1f;

            transform.position = target.transform.position + cmOffset;
            transform.LookAt(target.transform);
        }

    }

    void DebugFindKomaTag()//デバッグ用関数。
    {
        if (GameObject.FindGameObjectsWithTag("Koma").Length == 1)
        {
            target = GameObject.FindWithTag("Koma");
        }
    }

    void ChangeCamera()//カメラ切り替え関数
    {
        if (SpinController.isThrown == true)
        {
            cntTime += Time.deltaTime;
        }
        if (SpinController.isThrown && cntTime > 3)
        {
            gameObject.GetComponent<Camera>().depth = 10;//カメラの深度をプラスする
        }
    }
}