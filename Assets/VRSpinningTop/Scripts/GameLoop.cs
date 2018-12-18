using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{

    public SpinController spinController;//型名 変数名 (SpinController s)。gameObjectのSpinControllerとは違う
    public StringController stringController;//世界の中にあるgameObjectをここに入れる。
    public KinectController kinectController;//そうすることで、spinControllerの変数を使用することができる。
    public BodySourceView bodySourceView;
    public bool isHMD = true;
    private float afterTime = 0;//投げ終わってからの時間
    public GameObject Great;
    public GameObject adviseMoreFast;
    public GameObject adviseMoreSlow;
    public GameObject stringMachine;//Unity上のStringController
    private Vector3 windingDevice;
    public Vector3 wrist;

    public float stringLength;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject koma = bodySourceView.KomaObj;
        Rigidbody komaBody = koma.GetComponentInChildren<Rigidbody>();
        TimeCounter();
        WindingDistance();
        if (spinController.isThrown == true)//投げられたら。
        {
            //親子関係があると正常に動作しない
            koma.transform.parent = null;//親子関係を解除する

            Debug.Log("isThrown");
            komaBody.isKinematic = false;
            komaBody.useGravity = true;
            komaBody.velocity = spinController.velocity;
            komaBody.angularVelocity = Vector3.up * 3.14f;
            
            //koma.GetComponent<Rigidbody>().velocity = spinController.velocity;//スピンコントローラの速度を、コマに代入
        }
        //コマの速度が10以上20以下かつ、投げ終わってから3秒以上経過した場合
        if (10 <= komaBody.velocity.z  && komaBody.velocity.z <= 20 && 3 <= afterTime)
            //最終的には、リザルト画面で表示させる。
        {
            Debug.Log("もう速くヒモを引いてください！！");
            adviseMoreFast.SetActive(true);//アドバイステキストをアクティブにする
        }
        //コマの速度が1以上10以下かつ、投げ終わってから3秒以上経過した場合
        else if (1 <= komaBody.velocity.z && komaBody.velocity.z <= 10 && 3 <= afterTime)
        {
            Debug.Log("もうゆっくりヒモを引いてください！！");
            adviseMoreSlow.SetActive(true);//アドバイステキストをアクティブにする
        }
        else if (3 <= afterTime)
        {
            Debug.Log("素晴らしい");
            Great.SetActive(true);
        }

        /*if (Input.GetKeyDown("space"))
        {
            ForkParticlePlugin.Instance.Test();
        }*/
        /*if (Input.GetKeyDown(KeyCode.F))
        {

            komaBody.isKinematic = false;
            komaBody.useGravity = true;
            komaBody.angularVelocity = Vector3.up * 3.14f;
        }*/
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("touch Ground");
        }
    }
    void TimeCounter()//コマを投げ終わってからの時間を計る。
    {
        if (spinController.isThrown)//投げられたら。
        {
            afterTime += Time.deltaTime;
        }
    }

    public void WindingDistance()//巻取り距離
    {
        Vector3 windingDevice = stringMachine.transform.position;
        Vector3 wrist = kinectController.wristPosition;
        stringLength = Vector3.Distance(windingDevice, wrist);
        gameObject.GetComponentInChildren<StringController>().setTargetLength(stringLength);
    }
}
    
