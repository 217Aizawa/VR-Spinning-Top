using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    public enum GameState { free, preCalibration, calibration, spinInHand, spinInAir, result };
    public GameState gameState;
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

    //public Vector3 wrist;
    public Vector3 hand;

    public float stringLength;

    // Use this for initialization
    void Start()
    {
        gameState = GameState.free;
        stringController.setMotorMode(StringController.MotorMode.isFree);

    }

    // Update is called once per frame
    void Update()
    {
        GameObject koma = bodySourceView.KomaObj;
        Rigidbody komaBody = koma.GetComponentInChildren<Rigidbody>();
        afterTime += Time.deltaTime;
        switch (gameState)
        {
            // 体験開始時（紐はたるんでいる）
            case GameState.free:
                // タイトル画面を表示するならここ
                stringController.setMotorMode(StringController.MotorMode.isFree);

//               komaBody.rotation = spinController.g_rotation * Quaternion.AngleAxis(90, Vector3.left);
//                (GameObject.Find("f")).GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(-spinController.f.x, spinController.f.y, -spinController.f.z));

                if (Input.GetKeyDown(KeyCode.Space))
                {
//                    GameObject.Find("f").SetActive(false);
                    ChangeGameStateToNext();
                }
                break;
            // 紐巻取り（紐を十分短く）　利き手判定
            case GameState.preCalibration:
                stringController.setMotorMode(StringController.MotorMode.isRewinding);
                if (afterTime > 0.5)
                    ChangeGameStateToNext();
                break;
            // 紐を引きながらお客さんに手渡す
            case GameState.calibration:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    stringController.calibrateToLength(stringLength);
                    stringController.setMotorMode(StringController.MotorMode.isTrackingHand);
                    ChangeGameStateToNext();
                }

                if (Input.GetKey(KeyCode.A))//巻取りが足りなかったら巻き取る
                    stringController.setMotorMode(StringController.MotorMode.isRewinding);
                else
                    stringController.setMotorMode(StringController.MotorMode.isFree);

                break;
            case GameState.spinInHand:
                if (SpinController.isThrown == true)//投げられたら。
                {
                    stringController.setMotorMode(StringController.MotorMode.isShowingResistance);
                    ChangeGameStateToNext();
                    
                    //親子関係があると正常に動作しない
                    koma.transform.parent = null;//親子関係を解除する

                    Debug.Log("isThrown");
                    komaBody.isKinematic = false;
                    komaBody.useGravity = true;
                    komaBody.velocity = spinController.velocity;
                    komaBody.angularVelocity = Vector3.up * 3.14f;//追加
                    
                    //koma.GetComponent<Rigidbody>().velocity = spinController.velocity;//スピンコントローラの速度を、コマに代入
                }
                break;
            case GameState.spinInAir:
                // 投げ終わって３秒経過したら結果表示に
                if (afterTime > 3)
                {
                    stringController.setMotorMode(StringController.MotorMode.isFree);
                    ChangeGameStateToNext();

                    //コマの速度が10以上20以下かつ、投げ終わってから3秒以上経過した場合
                    if (10 <= komaBody.velocity.z && komaBody.velocity.z <= 20)
                    //最終的には、リザルト画面で表示させる。
                    {
                        //もう速くヒモを引いてください！！
                        adviseMoreFast.SetActive(true);//アドバイステキストをアクティブにする
                    }
                    //コマの速度が1以上10以下かつ、投げ終わってから3秒以上経過した場合
                    else if (1 <= komaBody.velocity.z && komaBody.velocity.z <= 10)
                    {
                        //もうゆっくりヒモを引いてください！！
                        adviseMoreSlow.SetActive(true);
                    }
                    else
                    {
                        //素晴らしい
                        Great.SetActive(true);
                    }
                }
                break;
            case GameState.result:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ResetScene();//追加
                    gameState = GameState.free;
                    GameObject.Find("f").SetActive(true);
                }
                break;
        }
        
        WindingDistance();

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
    
    public void WindingDistance()//巻取り距離
    {
        windingDevice = stringMachine.transform.position;
        //wrist = bodySourceView.handednessHandPos;//kinectController.wristPosition
        hand = bodySourceView.handednessHandPos;
        //Debug.Log("wrist" + wrist);
        //stringLength = Vector3.Distance(windingDevice, wrist);
        stringLength = Vector3.Distance(windingDevice, hand);
        //Debug.Log(stringLength);
        stringController.setTargetLength(stringLength);
    }

    void ChangeGameStateToNext()
    {
        afterTime = 0;
        int currentState = (int)gameState;
        currentState = (currentState + 1) % Enum.GetNames(typeof(GameState)).Length;
        gameState = (GameState)Enum.ToObject(typeof(GameState), currentState);
    }

    void ResetScene()
    {
        SceneManager.LoadScene("VRSpinningTop");
    }
}
