using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public SpinController spinController;
    public PhysicMaterial physicMaterial;

    private void Awake()
    {
        physicMaterial.dynamicFriction = 0;
        physicMaterial.staticFriction = 0;
        physicMaterial.bounciness = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spinController.isThrown == true)//Input.GetKeyDown(KeyCode.F)
        {
            GetComponent<Animator>().SetTrigger("SampleTrigger");
            physicMaterial.dynamicFriction = 1;

        }
    }
}
