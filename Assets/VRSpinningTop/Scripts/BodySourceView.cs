using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityEngine.XR;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject gameLoop;
    public GameObject Camera;
    public GameObject Koma;
    //GameObject WristKoma;
    GameObject HandKoma;
    public GameObject KomaObj;
    public GameObject Text;//3Dテキスト

    public Vector3 headPos;//追加
    public Vector3 handLeftPos;//左手位置
    public Vector3 handRightPos;//右手位置
    //public Vector3 handednessWristPos;//利き手の手首位置

    public Vector3 handednessHandPos;//利き手位置　追加

    private float leftHandTime = 0;//左手を挙げている時間
    private float rightHandTime = 0;


    public int handedness;//追加 public
    bool riseHand = true;//利き手判定用に一度だけ判定するbool。ゲームが終了したら、trueに戻す。


    private int trackedId = -1;//-1は検出できていない状態
    public Vector3 OffsetToWorld = Vector3.zero;//publicにすると外から参照できる


    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    //kinect.setMirror(false);
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Start()
    {
        
    }

  
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Koma").Length == 1)
        {
            KomaObj = GameObject.FindWithTag("Koma");
        }

        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }
        Vector3 closestPosition = Vector3.zero;
        GameLoop gl = gameLoop.GetComponent<GameLoop>();

        //foreach(var body in data) ボディ取り出し
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == null)
            {
                continue;
            }

            if (data[i].IsTracked)
            {
                //get kinect coodinate without offset　[i] ボディ型、Joints{  } JointTypeからJointへの辞書　JointはVector3のようなもの
                headPos = GetVector3FromJointWithOffset(data[i].Joints[Kinect.JointType.Head]);
                handLeftPos = GetVector3FromJointWithOffset(data[i].Joints[Kinect.JointType.HandLeft]);
                handRightPos = GetVector3FromJointWithOffset(data[i].Joints[Kinect.JointType.HandRight]);

                // found new body
                if (!_Bodies.ContainsKey(data[i].TrackingId))
                {
                    _Bodies[data[i].TrackingId] = CreateBodyObject(data[i].TrackingId);
                }

                if (Mathf.Abs(headPos.x) < 0.3f)//&& Mathf.Abs(position.z) < 1.5f)//xの絶対値が両側30センチ＆zは絶対値を取らなくてよい。人が居たら開始
                {
                    /*if (!gl.isHMD)
                    {
                        gl.NextStateWithCheckCurrentState(GameLoop.GameState.Opening);//オープニング状態なら次へ
                    }*/
                }

                // closest player detection
                if (Mathf.Abs(headPos.x) < 0.6f)//&& gl.state != GameLoop.GameState.End)(1.0f)
                {
                    if (closestPosition == Vector3.zero || Mathf.Abs(closestPosition.z) > Mathf.Abs(headPos.z))
                    {
                        trackedId = i;//IDを割り当てる
                        closestPosition = headPos;//一番近い人を覚える
                        Debug.Log("trackedId" + " = " + trackedId);
                    }
                }
            }
            else
            {
                if (trackedId == i)
                    trackedId = -1;
            } 
        }

        //VR画面で使う
        if (trackedId != -1 && data[trackedId] != null)//Kinectで認識したら
        {
            RefreshBodyObject(data[trackedId], _Bodies[data[trackedId].TrackingId]);//body

            LeftHandCounter();//挙手時間計測
            RightHandCounter();

            //利き手判定スクリプト（左手）
            if (riseHand == true && headPos.y < handLeftPos.y && 3 <= leftHandTime)
            {
                Text.SetActive(false);
                //WristKoma = GameObject.Find("WristLeft");
                HandKoma = GameObject.Find("HandLeft");
                handedness = -1;
                riseHand = false;
                CreatePrefab();//子としてコマを生成する
                KomaObj.transform.localPosition = Vector3.zero;//検証
                Debug.Log("Handedness Left");
            }
            else if (riseHand == true && headPos.y < handRightPos.y && 3 <= rightHandTime)
            {
                Text.SetActive(false);
                //WristKoma = GameObject.Find("WristRight");
                HandKoma = GameObject.Find("HandRight");
                handedness = 1;
                riseHand = false;
                CreatePrefab();
                KomaObj.transform.localPosition = Vector3.zero;//検証
                Debug.Log("Handedness Right");
            }
            //利き手の手首位置を返し続けるためのif文
            if (handedness == -1)
            {
                //handednessWristPos = GetVector3FromJointWithOffset(data[trackedId].Joints[Kinect.JointType.WristLeft]);
                handednessHandPos = handLeftPos;//左手を利き手に代入
            }
            else if (handedness == 1)
            {
                //handednessWristPos = GetVector3FromJointWithOffset(data[trackedId].Joints[Kinect.JointType.WristRight]);
                handednessHandPos = handRightPos;//右手を利き手に代入
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))//利き手強制切り替えスクリプト（テンキーのEnterキー）
            {
                Debug.Log("GetKeyDown Enter");
                if (handedness == -1)//左手なら
                {
                    handedness = 1;
                    Debug.Log("Converted RightHandedness");
                }
                else if (handedness == 1)//右手なら
                {
                    handedness = -1;
                    Debug.Log("Converted LeftHandedness");
                }
            }

            //キーボードで利き手切り替え
            if (Input.GetKeyDown(KeyCode.L))
            {
                handedness = -1;
                //WristKoma = GameObject.Find("WristLeft");
                HandKoma = GameObject.Find("HandLeft");

                Debug.Log("Converted LeftHandedness");
                //KomaObj.transform.parent = WristKoma.transform;//正常に動作する
                KomaObj.transform.parent = HandKoma.transform;

                KomaObj.transform.localPosition = Vector3.zero;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                handedness = 1;
                //WristKoma = GameObject.Find("WristRight");
                HandKoma = GameObject.Find("HandRight");

                Debug.Log("Converted RightHandedness");
                //KomaObj.transform.parent = WristKoma.transform;
                KomaObj.transform.parent = HandKoma.transform;

                KomaObj.transform.localPosition = Vector3.zero;
            }
            // Get the head position without offsetting to Oculus
            // and use it to determine the offset
            Vector3 posHeadKinect = GetVector3FromJoint(data[trackedId].Joints[Kinect.JointType.Head]);
            if (gl.isHMD)
            {
                Vector3 posOculus = Camera.transform.position;
                OffsetToWorld = posOculus - posHeadKinect;//Oculusの位置を基準にKinectの座標をずらす。
                //Debug.Log(posOculus + "/" + posHeadKinect + "/" + OffsetToWorld);//座標表示
            }
            else
            {
                Debug.Log("VR mode FALSE");
                Camera.transform.position = posHeadKinect;
            }
            //sendSkeleton(data[trackedId]);
        }
    }

    private Vector3 GetVector3FromJoint(Kinect.Joint joint)//座標を返す関数 KinectのJointを受け取る
    {
        Vector3 localPosition = new Vector3(joint.Position.X * 1, joint.Position.Y * 1, joint.Position.Z * -1);//KinectとUnityの世界は約10倍違う(メートルに直す)
        Vector3 globalPosition = gameObject.transform.TransformPoint(localPosition);//Kinect座標をグローバル(Unity)座標に変換

        return globalPosition;

        //return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }


    private Vector3 GetVector3FromJointWithOffset(Kinect.Joint joint)//追加　Jointを持ってくる 
    {
        Vector3 globalPosition = GetVector3FromJoint(joint);

        GameLoop gl = gameLoop.GetComponent<GameLoop>();
        /*        if (!gl.isHMD)
                {
                    localPosition.x *= gl.SpreadFactor;
                    localPosition.y *= gl.SpreadFactor;
                }
        */
        //Debug.Log("globalPosition" + globalPosition);

        globalPosition += OffsetToWorld;//グローバルポジションをオフセット分ずらす。
            
        return globalPosition;
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);


            
            jointObj.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);//KinectBodyの大きさを設定できる 0.3f, 0.3f, 0.3f
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;//body.transform

        }

        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJointWithOffset(sourceJoint);//Bodyを表示
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJointWithOffset(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    


    /*****************************************************************************************************************************
     * 追加
     * 
     */
     
    public void LeftHandCounter()//左手が上がっている時間をカウントする関数
    {
        if(headPos.y < handLeftPos.y)
        {
            leftHandTime += Time.deltaTime;//毎フレームの時間を加算する
            Debug.Log("LeftCount Start");

            rightHandTime -= Time.deltaTime;//もう一方の挙手時間を減らす。そうすることで両手を上げても時間を相殺しあう。
        }
    }

    public void RightHandCounter()//右手が上がっている時間をカウントする関数
    {
        if(headPos.y < handRightPos.y)
        {
            rightHandTime += Time.deltaTime;//毎フレームの時間を加算する
            Debug.Log("RightCount Start");
            leftHandTime -= Time.deltaTime;
        }

    }

    void CreatePrefab()//コマプレハブ生成関数
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Koma");

        if (obj != null)
            Destroy(obj);
        //Instantiate(Object, Vector3, Quaternion, Parent.transform)
        //KomaObj = (GameObject)Instantiate(Koma, handednessWristPos, Quaternion.identity, WristKoma.transform);
        KomaObj = (GameObject)Instantiate(Koma, handednessHandPos, Quaternion.identity, HandKoma.transform);

        KomaObj.transform.SetParent(transform, false);//親のスケールの影響を受けないようにするコード。
        //KomaObj.transform.parent = WristKoma.transform;
        KomaObj.transform.parent = HandKoma.transform;

    }
    //Aizaws Branch
}