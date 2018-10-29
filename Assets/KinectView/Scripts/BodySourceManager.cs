using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    
    public Body[] GetData()
    {
        Debug.Log(_Data);
        return _Data;
    }
    

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }   
    }
    
    void Update () 
    {
        if (_Reader != null)//フレーム取得
        {
            var frame = _Reader.AcquireLatestFrame();
            
            if (frame != null)
            {
                
                if (_Data == null)//ボディを取得
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                    Debug.Log(_Data);

                }
                
                frame.GetAndRefreshBodyData(_Data);
                Debug.Log(_Data.Length);

                frame.Dispose();
                frame = null;
                Debug.Log(_Data.Length);
            }
        }    
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
