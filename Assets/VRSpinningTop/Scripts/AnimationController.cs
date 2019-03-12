using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public SpinController spinController;
    public KomaPhysics komaPhysics;//SpinningTopComplete2(1)
    Rigidbody rb;
    public Animator anim;
    private float countTime = 0;//タイマー

    // Start is called before the first frame update
    void Start()
    {
        //rb = komaPhysics.GetComponent<Rigidbody>();//komaPhysicsのRbを取得 親用
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();//アニメーターセット

        //PrefabはSpinControllerがセットされていないで呼び出される。
    }

    // Update is called once per frame
    void Update()
    {
        if(SpinController.isThrown == true)
            countTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //anim.SetTrigger("Fail");
            //anim.SetTrigger("Success");
            rb.constraints = RigidbodyConstraints.FreezePosition;//ポジション固定
        }

        if (SpinController.isThrown == true && countTime > 1)//isThrownかつ投げてから1秒以上経過していれば
        {
            //anim.SetTrigger("Fail");
            //anim.SetTrigger("Success");
        }

        //本番用判定
        
    }

    void OnCollisionEnter(Collision collision)//Rigidbodyがないと衝突検知できない
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
