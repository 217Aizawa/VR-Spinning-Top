using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public static Camera SubCamera;
    public GameObject mainCamera;
    public static float cntTime;
    Vector3 cmOffset;
    public Vector3 Position;
    public GameLoop gl;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        ChangeCamera();
        FindKomaTag();
        Position = transform.position;
    }

    void FindKomaTag()//本番用
    {
        if (GameObject.FindGameObjectsWithTag("KomaChild").Length == 1)
        {
            GameObject target;
            target = GameObject.FindWithTag("KomaChild");

            cmOffset = mainCamera.transform.position - target.transform.position;

            cmOffset.Normalize();

            //cmOffset = cmOffset * 0.5f;//0.2 VRmode 1.5f

            //本番用 isJudge.ver
            
            if (JudgeController.isJudge || gl.isHMD)//VRmodeならばcmoOffsetを1.5に
                cmOffset = cmOffset * 1f; 
            else
                cmOffset = cmOffset * 0.5f;//PROJECTORmodeならばcmOffsetを0.5に
            
            cmOffset.y = 0.1f;
            //Debug.Log("cmOffset " + "/" + cmOffset);

            Vector3 arrow = target.transform.position + cmOffset - mainCamera.transform.position;//サブカメラにいて欲しい座標
            transform.parent.transform.position = arrow;
            transform.LookAt(target.transform);
        }
    }

    void ChangeCamera()//カメラ切り替え関数
    {
        if (gl.gameState == GameLoop.GameState.result)//リザルト表示なら
        {
            gameObject.GetComponent<Camera>().depth = 10;//カメラの深度をプラスする

        }
    }
}