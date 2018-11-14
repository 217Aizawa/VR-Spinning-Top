using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    public Vector3 headPos;//追加
    public Vector3 handLeftPos;//左手位置
    public Vector3 handRightPos;//右手位置
    public Vector3 handednessWristPos;//利き手の手首位置

    private float leftHandTime = 0;//左手を挙げている時間
    private float rightHandTime = 0;


    public int handedness;//追加 public
    bool riseHand = true;//利き手判定用に一度だけ判定するbool。ゲームが終了したら、trueに戻す。


    private int trackedId = -1;//-1は検出できていない状態
    private Vector3 OffsetToWorld = Vector3.zero;//publicにすると外から参照できる


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
        //GameLoop gl = gameLoop.GetComponent<GameLoop>();

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
                headPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.Head], false);
                handLeftPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.HandLeft], false);
                handRightPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.HandRight], false);

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

                // closest player detection　人を一人選ぶ
                if (Mathf.Abs(headPos.x) < 0.6f)//&& gl.state != GameLoop.GameState.End)(1.0f)
                {
                    if (closestPosition == Vector3.zero || Mathf.Abs(closestPosition.z) > Mathf.Abs(headPos.z))
                    {
                        trackedId = i;
                        closestPosition = headPos;//より近くに人が来たら記憶する
                    }
                }
                RefreshBodyObject(data[i], _Bodies[data[i].TrackingId]);

                LeftHandCounter();//挙手時間計測
                RightHandCounter();

                //利き手判定スクリプト（左手）
                if (riseHand == true && headPos.y < handLeftPos.y && 3 <= leftHandTime)
                {
                    handedness = -1;
                    riseHand = false;
                    Debug.Log("Handedness Left");
                }
                else if (riseHand == true && headPos.y < handRightPos.y && 3 <= rightHandTime)
                {
                    handedness = 1;
                    riseHand = false;
                    Debug.Log("Handedness Right");
                }

                //手首位置取得スクリプト
                if (handedness == -1)//左
                {
                    handednessWristPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.WristLeft], false);
                    //Debug.Log("Confirm LeftWrist");

                }
                else if (handedness == 1)//右
                {
                    handednessWristPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.WristRight], false);
                    //Debug.Log("Confirm RightWrist");

                }

                if (Input.GetKeyDown(KeyCode.KeypadEnter))//利き手強制切り替えスクリプト（テンキーのEnterキー）
                {
                    Debug.Log("GetKeyDown Enter");
                    if(handedness == -1)//左手なら
                    {
                        handedness = 1;
                        Debug.Log("Converted RightHandedness");
                    }
                    else if(handedness == 1)//右手なら
                    {
                        handedness = -1;
                        Debug.Log("Converted LeftHandedness");
                    }
                }

                //キーボードで利き手切り替え
                if (Input.GetKeyDown(KeyCode.L))
                {
                    handedness = -1;
                    Debug.Log("LKey Down");
                    Debug.Log("Converted LeftHandedness");
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    handedness = 1;
                    Debug.Log("RKey Down");
                    Debug.Log("Converted RightHandedness");
                }

            }
            else
            {
                if (trackedId == i)
                    trackedId = -1;
            }
            
        }

        //VR画面で使う
        /*if (trackedId != -1 && data[trackedId] != null)
        {
            // Get the head position without offsetting to Oculus
            // and use it to determine the offset
            Vector3 posHeadKinect = GetVector3FromJoint(data[trackedId].Joints[Kinect.JointType.Head], false);
            if (gl.isHMD)
            {
                Vector3 posOculus = MainCamera.transform.position;
                OffsetToWorld = posOculus - posHeadKinect;//Oculusの位置を基準にKinectの座標をずらす。
            }
            else
            {
                //OffsetToWorld = Vector3.zero;
                //OffsetToWorld.x = data[trackedId].Joints[Kinect.JointType.Head].Position.X * 10;
                MainCamera.transform.position = posHeadKinect;
            }
            sendSkeleton(data[trackedId]);
        }*/
    }
    private Vector3 GetVector3FromJoint(Kinect.Joint joint, bool applyOffet = true)//追加　Jointを持ってくる
    {
        Vector3 localPosition = new Vector3(joint.Position.X, joint.Position.Y, -joint.Position.Z);

        //GameLoop gl = gameLoop.GetComponent<GameLoop>();
        /*        if (!gl.isHMD)
                {
                    localPosition.x *= gl.SpreadFactor;
                    localPosition.y *= gl.SpreadFactor;
                }
        */
        Vector3 globalPosition = gameObject.transform.TransformPoint(localPosition);//Kinect座標をグローバル(Unity)座標に変換

        if (applyOffet)
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


            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);//KinectBodyの大きさを設定できる
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
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
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
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)//座標を返す関数 KinectのJointを受け取る
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * -1);//KinectとUnityの世界は約10倍違う(メートルに直す)
        //return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
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

    /*
    void JudgeHandedness()
    {
        if (headPos.y < handLeftPos.y)
        {
            handedness = -1;
        }
        else if (headPos.y < handRightPos.y)
        {
            handedness = 1;
        }

        switch (handedness)
        {
            case -1:
                Debug.Log("Handedness Left");
                break;

            case 1:
                Debug.Log("Handedness Right");
                break;
        }
    }*/

    /*void Update () 
  {

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

      foreach(var body in data)//データの中のボディを使う 一旦コメントアウト
      {
          if (body == null)
          {
              continue;
          }

          if(body.IsTracked)
          {
              trackedIds.Add (body.TrackingId);
          }
      }

      Vector3 closestPosition = Vector3.zero;
      //GameLoop gl = gameLoop.GetComponent<GameLoop>();

      //foreach(var body in data)ボディ取り出し 追加
      for (int i = 0; i < data.Length; i++)
      {
          if (data[i] == null)
          {
              continue;
          }

          if (data[i].IsTracked)
          {
              //get kinect coodinate without offset　[i] ボディ型、Joints{  } JointTypeからJointへの辞書　JointはVector3のようなもの
              headPos = GetVector3FromJoint(data[i].Joints[Kinect.JointType.Head], false);////頭の位置をジョイントから持ってくる

              // found new body
              if (!_Bodies.ContainsKey(data[i].TrackingId))
              {
                  _Bodies[data[i].TrackingId] = CreateBodyObject(data[i].TrackingId);
              }

              if (Mathf.Abs(headPos.x) < 0.3f )//&& Mathf.Abs(position.z) < 1.5f)//xの絶対値が両側30センチ＆zは絶対値を取らなくてよい。人が居たら開始
              {
                  /*if (!gl.isHMD)
                  {
                      gl.NextStateWithCheckCurrentState(GameLoop.GameState.Opening);//オープニング状態なら次へ
                  }
              }

              // closest player detection　人を一人選ぶ
              if (Mathf.Abs(headPos.x) < 1.0f )//&& gl.state != GameLoop.GameState.End)
              {
                  if (closestPosition == Vector3.zero || Mathf.Abs(closestPosition.z) > Mathf.Abs(headPos.z))//より近くに人が来たら記憶する
                  {
                      trackedId = i;
                      closestPosition = headPos;
                  }
              }
              RefreshBodyObject(data[i], _Bodies[data[i].TrackingId]);
          }
          else
          {
              if (trackedId == i)
                  trackedId = -1;
          }
      }

      List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

      // First delete untracked bodies
      foreach(ulong trackingId in knownIds)
      {
          if(!trackedIds.Contains(trackingId))
          {
              Destroy(_Bodies[trackingId]);
              _Bodies.Remove(trackingId);
          }
      }        
  }*/

}