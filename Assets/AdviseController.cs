using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdviseController : MonoBehaviour
{
    public GameLoop gl;
    public SpinController sc;

    //投げ出し、引く速さ、引き始めの順番
    public GameObject Advise1;//速、速、速

    public GameObject Advise2;//速、速、遅

    public GameObject Advise3;//速、遅、速

    public GameObject Advise4;//速、遅、遅

    public GameObject Advise5;//遅、速、速

    public GameObject Advise6;//遅、速、遅

    public GameObject Advise7;//遅、遅、速

    public GameObject Advise8;//遅、遅、遅

    public GameObject Great;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Vkoma = sc.velocity;
        Vkoma.z = 0;
        float komaSpeed = Vkoma.magnitude;

        if (gl.gameState == GameLoop.GameState.result)
            timer = Time.deltaTime;
        if(timer > 3)
        {
            if (1.6 <= komaSpeed && komaSpeed <= 3.4)
            {
                //Great
                Debug.Log("KomaSpeed" + komaSpeed);
                //Great.SetActive(true);
            }
            else if (3.4 <= komaSpeed)
            {
                //速すぎる
                //adviseMoreSlow
                Debug.Log("KomaSpeed" + komaSpeed);
            }
            else
            {
                //遅すぎる
                //adviseMoreFast
                Debug.Log("KomaSpeed" + komaSpeed);
            }
        }
    }
}
