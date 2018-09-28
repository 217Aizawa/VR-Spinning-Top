using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct EncoderCount
{
    public int count;
    public float dt;
}

public class EncoderController : MonoBehaviour {
    List<EncoderCount> samples;
    int formerCount;
    int currentCount;
    
	// Use this for initialization
	void Start () {
        samples = new List<EncoderCount>();
        resetCounter();
	}
	
	// Update is called once per frame
	void Update () {
        currentCount = updateCount(currentCount);

        EncoderCount ec;
        ec.dt = Time.deltaTime;
        ec.count = (currentCount - formerCount);
        samples.Add(ec);
        if( samples.Count > 10)
        {
            samples.RemoveAt(0);
        }

        formerCount = currentCount;
	}

    public void resetCounter()
    {
        currentCount = 0;
        formerCount = 0;
    }

    public float getSpeed()
    {
        EncoderCount sum;
        sum.count = 0;
        sum.dt = 0;

        foreach(EncoderCount item in samples)
        {
            sum.count += item.count;
            sum.dt += item.dt;
            if (sum.count > 5)
                break;
        }
        return sum.count / sum.dt;
    }

    int updateCount(int c)
    {
        return c + 1;
    }
}
