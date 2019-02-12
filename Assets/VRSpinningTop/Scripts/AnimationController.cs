using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public SpinController spinController;
    public PhysicMaterial physicMaterial;
    Rigidbody rb;

    private void Awake()
    {
        ResetPhysics();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
            GetComponent<Animator>().SetTrigger("Success");


        if (spinController.isThrown == true)//Input.GetKeyDown(KeyCode.F)
        {
            //GetComponent<Animator>().SetTrigger("SampleTrigger");
            //physicMaterial.dynamicFriction = 1;
            //physicMaterial.staticFriction = 1;
        }
    }

    void ResetPhysics()
    {
        physicMaterial.dynamicFriction = 0;
        physicMaterial.staticFriction = 0;
        physicMaterial.bounciness = 0;
    }
}
