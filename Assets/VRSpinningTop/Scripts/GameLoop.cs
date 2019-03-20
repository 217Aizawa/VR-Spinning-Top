using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameLoop : MonoBehaviour
{
    public enum GameState { free, preCalibration, calibration, spinInHand, spinInAir, result };

    public GameState gameState = GameState.free;
    public SpinController spinController;//型名 変数名 (SpinController s)。gameObjectのSpinControllerとは違う
    public StringController stringController;//世界の中にあるgameObjectをここに入れる。
    public KinectController kinectController;//そうすることで、spinControllerの変数を使用することができる。
    public BodySourceView bodySourceView;
    public AnimationController animationController;
    public bool isHMD = true;
    private float afterTime = 0;//投げ終わってからの時間
    bool Success;

    public GameObject Camera;

    //投げ出し、引く速さ、引き始めの順番
    public GameObject VisorHUD;
    public GameObject Image;
    public GameObject ThrowF;
    public GameObject ThrowS;
    public GameObject PullSpeedF;
    public GameObject PullSpeedS;
    public GameObject PullStartF;
    public GameObject PullStartS;
    public GameObject Great;

    public GameObject stringMachine;//Unity上のStringController
    private Vector3 windingDevice;

    //public Vector3 wrist;
    public Vector3 hand;

    public float stringLength;

    public bool isDebugging;


    private void Awake()
    {
        Judge();
        //ModeSelect(); //デバッグ用
        if (JudgeController.isJudge == true && JudgeController.isHeight != null )
        {
            Debug.Log("Done");
            bodySourceView.KomaObj.transform.position = Camera.transform.position + new Vector3(0, 0.6f, 0.3f);//0.5
        }
        else if (JudgeController.isHeight == "tall")//PROJECTOR
        {
            bodySourceView.KomaObj.transform.position = new Vector3(0, 2.05f, 1);
        }
        else if (JudgeController.isHeight == "middle")
        {
            bodySourceView.KomaObj.transform.position = new Vector3(0, 1.75f, 1);

        }
        else if (JudgeController.isHeight == "short")
        {
            bodySourceView.KomaObj.transform.position = new Vector3(0, 1.32f, 1);

        }
        
    }

    // Use this for initialization
    void Start()
    {
        gameState = GameState.free;
        stringController.setMotorMode(StringController.MotorMode.isFree);
    }

    // Update is called once per frame
    void Update()
    {
        //XRSettings.renderViewportScale = 0.5f;
        GameObject koma = bodySourceView.KomaObj;
        Rigidbody komaBody = koma.GetComponentInChildren<Rigidbody>();
        afterTime += Time.deltaTime;

        switch (gameState)
        {
            // 体験開始時（紐はたるんでいる）
            case GameState.free:
                // タイトル画面を表示するならここ

                //komaBody.rotation = spinController.g_rotation * Quaternion.AngleAxis(90, Vector3.left);
                //(GameObject.Find("f")).GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(-spinController.f.x, spinController.f.y, -spinController.f.z));

                if (Input.GetKeyDown(KeyCode.Space) || bodySourceView.handedness != 0)
                {
                    ChangeGameStateToNext();
                }
                stringController.setMotorMode(StringController.MotorMode.isFree);
                break;
            // 紐巻取り（紐を十分短く）　利き手判定
            case GameState.preCalibration:
                stringController.setMotorMode(StringController.MotorMode.isTrackingHand);   // keep tension
                if (afterTime > 0.5)
                    ChangeGameStateToNext();
                break;
            // 紐を引きながらお客さんに手渡す
            case GameState.calibration:
                if (bodySourceView.handedness != 0)
                    GameObject.FindGameObjectWithTag("KomaChild").transform.position = bodySourceView.handednessHandPos;

                if (Input.GetKeyDown(KeyCode.Space))// || bodySourceView.handedness != 0)//利き手判定後にも遷移できるように
                {
                    if (bodySourceView.handedness == 0)
                        bodySourceView.handedness = 1;
                    stringController.calibrateToLength(stringLength);
                    ChangeGameStateToNext();
                }
                else
                {
                    stringController.setMotorMode(StringController.MotorMode.isTrackingHand);
                }

                if (Input.GetKey(KeyCode.Backspace))//JudgeSceneに遷移
                    SceneManager.LoadScene("JudgeScene");
                if (Input.GetKey(KeyCode.V))
                {
                    SpinController.isThrown = true;
                    Debug.Log("IsThrown True");
                }
                SpinController.isThrown = false;
                break;
            case GameState.spinInHand:
                //animationController.anim.SetTrigger("Idle");
                GameObject.FindGameObjectWithTag("KomaChild").transform.position = bodySourceView.handednessHandPos;

                if (SpinController.isThrown == true || Input.GetKeyDown(KeyCode.Space) )//投げられたら。
                {
                    SpinController.isThrown = true; // スペースキーで遷移したときに強制的に投げた状態にする
                    stringController.setMotorMode(StringController.MotorMode.isShowingResistance);
                    ChangeGameStateToNext();

                    //isThrownの1秒後にアニメーション再生する。

                    //親子関係があると正常に動作しない
                    koma.transform.parent = null;//親子関係を解除する

                    //Debug.Log("isThrown");
                    komaBody.isKinematic = false;
                    komaBody.useGravity = true;
                    komaBody.velocity = spinController.velocity;
                    komaBody.angularVelocity = Vector3.up * 3.14f;//追加
                    
                    //koma.GetComponent<Rigidbody>().velocity = spinController.velocity;//スピンコントローラの速度を、コマに代入
                }

                if (Input.GetKey(KeyCode.Backspace))//JudgeSceneに遷移
                    SceneManager.LoadScene("JudgeScene");

                break;
            case GameState.spinInAir:
   
                //コマの成功・失敗判定のパラメータ
                

                // 投げ終わって2秒経過　or 糸を引ききったら結果表示に0.3
                if (afterTime > 3 || stringController.isPulling == false)
                {
                    if (isHMD)
                        VisorHUD.transform.localPosition = new Vector3(0,0,0.8f);
                    else
                        VisorHUD.transform.localPosition = new Vector3(0, 0, 0.5f);

                    Image.SetActive(true);

                    Vector3 Vkoma = spinController.velocity;
                    //Vkoma.z = 0;
                    float komaSpeed = Vkoma.magnitude;//ベクトルの長さを返す。
                    float komaRotationSpped = spinController.rotationSpeedZ;
                    float komaAngle = spinController.angleZ;
                    float pullTimeStartUp = stringController.timeStartup;
                    float pullTimeTotal = stringController.timeTotal;
                    float pullSpeed = stringController.maxPullingSpeed;
                    Debug.Log("KomaSpeed" + ":" + komaSpeed + " / " +"RotationSpeed " + ":" + komaRotationSpped + " " + "Angle" + ":" + komaAngle + "\n"
                        +  "TimeStartUp" + ":" + pullTimeStartUp + "/ " + "TimeTotal : " + pullTimeTotal + " / " + "Speed : " + pullSpeed);


                    stringController.setMotorMode(StringController.MotorMode.isFree);
                    ChangeGameStateToNext();

                    Success = true;
                    
                    if (3.0f <= komaSpeed)//速すぎる
                    {
                        animationController.FailAnim();//失敗時のアニメーション
                        ThrowS.SetActive(true);
                        Success = false;
                    }
                    else if (komaSpeed < 1.2f)//遅すぎる
                    {
                        animationController.FailAnim();
                        ThrowF.SetActive(true);
                        Success = false;
                    }
                    
                    if (0.75 < pullTimeStartUp)//遅すぎ0.2
                    {
                        animationController.FailAnim();
                        PullStartF.SetActive(true);
                        Success = false;
                    }
                    else if (pullTimeStartUp < 0.4)//速すぎ0.3
                    {
                        animationController.FailAnim();
                        PullStartS.SetActive(true);
                        Success = false;
                    }

                    if (3500 < pullSpeed)//速すぎ
                    {
                        animationController.FailAnim();
                        PullSpeedS.SetActive(true);
                        Success = false;
                    }
                    else if (pullSpeed < 1000)//遅すぎ
                    {
                        animationController.FailAnim();
                        PullSpeedF.SetActive(true);
                        Success = false;
                    }
                    
                    if (Success == true)//成功
                    {
                        animationController.SuccessAnim();
                        spinController.SetSuccessEffect(0);
                        Great.SetActive(true);
                    }

                }
                break;
            case GameState.result:
                stringController.setMotorMode(StringController.MotorMode.isRewinding);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameState = GameState.free;     // ResetScene でシーンがリロードされるので、実際には不要
                    ResetScene();//追加
                }
                if (Input.GetKeyDown(KeyCode.A) || isDebugging )
                {
                    gameState = GameState.preCalibration;
                    //gameState = GameState.spinInHand;//デバッグ用
                    //animationController.anim.StopPlayback();
                    TurnOffAdivces();
                    spinController.StopSuccessEffect();
                    //コマを利き手の子にする。
                    bodySourceView.InstantiateKoma();
                    komaBody.constraints = RigidbodyConstraints.None;
                    spinController.ResetSpin();
                    stringController.setMotorMode(StringController.MotorMode.isTrackingHand);
                    animationController.IdleAnim();
                }
                break;
        }
        
        //WindingDistance();

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

    void TurnOffAdivces()
    {
        Image.SetActive(false);
        ThrowF.SetActive(false);
        ThrowS.SetActive(false);
        PullSpeedF.SetActive(false);
        PullSpeedS.SetActive(false);
        PullStartF.SetActive(false);
        PullStartS.SetActive(false);
        Great.SetActive(false);
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("touch Ground");
        }
    }*/
    
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
        // Debug.Log("New Game State: " + gameState);
    }

    void ResetScene()
    {
        SceneManager.LoadScene("VRSpinningTop");
    }

    void Judge()//JudgeSceneのキー入力を基に表示方法を切り替える
    {
        if (JudgeController.isJudge)
        {
            isHMD = JudgeController.isJudge;
            XRSettings.enabled = true;

        }else
        {
            isHMD = false;
            XRSettings.enabled = false;
        }
    }

    void ModeSelect()//VRSpininngTopシーン内の確認用関数。本番では使用しない。
    {
        if (isHMD)
        {
            XRSettings.enabled = true;
        }
        else
        {
            XRSettings.enabled = false;
        }
    }

}
