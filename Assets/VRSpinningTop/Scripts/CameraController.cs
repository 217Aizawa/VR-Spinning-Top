using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera SubCamera;
    private float cntTime;
    Vector3 cmOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Counter();
        ChangeCamera();
        /*FindKomaTag();
        GameObject.FindGameObjectsWithTag("Koma");//Komaタグがついているオブジェクトを探す。*/
    }

    void FindKomaTag()
    {
        if (GameObject.FindGameObjectsWithTag("KomaChild").Length == 1)
        {
            GameObject.FindWithTag("KomaChild");
            Debug.Log("Find KomaChild");
        }
    }
    
    void Counter()
    {
        
    }
    void ChangeCamera()
    {
        if (SpinController.isThrown == true)
        {
            cntTime += Time.deltaTime;
        }
        if (SpinController.isThrown && cntTime > 3)
        {
            SubCamera.depth = 3;//カメラの深度を3プラスする

        }
    }
}