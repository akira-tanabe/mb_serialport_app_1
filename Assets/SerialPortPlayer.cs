using System;
using UnityEngine;
using System.Collections.Generic;

public class SerialPortPlayer : MonoBehaviour
{
    [SerializeField] string portname = "/dev/cu.usbmodem000";
    [SerializeField] SerialPortReciever receiver;

    void Awake()
    {
        receiver?.Dispose();
        receiver = SerialPortReciever.Create(portname);
        receiver.onreceiveaction = onreceive;
    }

    void OnDestroy()
    {
        receiver?.Dispose();
        receiver = null;
    }
    string getvalue(string m, string ifempty = "0") => (string.IsNullOrEmpty(m))? ifempty : m;
    int getintvalue(string m, string ifempty = "0") => int.Parse((string.IsNullOrEmpty(m)) ? ifempty : m);
    void onreceive(string m)
    {
        if (!string.IsNullOrEmpty(m))
        {
            var ms = m.Split(",");
            if (ms.Length < 5) new Exception();
            //
            var ex = $"\"press\":{getintvalue(ms[0])}," +
                $"\"axisx\":{(float)getintvalue(ms[1])}," +
                $"\"axisy\":{(float)getintvalue(ms[2])}," +
                $"\"axisz\":{(float)getintvalue(ms[3])}," +
                $"\"strength\":{getintvalue(ms[4])}";
            //$"\"gesture\":\"{getvalue(ms[5],"")}\"," +
            //$"\"gest_histry\":{getvalue(ms[6],"\"\"")}";
            // json化
            ex = "{"+ex+"}";
            //m = m.Replace("'", "\"");
            //m = m.Replace("[]", "[\"\"]");
            //print(m+"\n"+ex);
            try
            {
                history.Insert(0,JsonUtility.FromJson<DeviceContext>(ex));
                if (history.Count > MAXHISTORY) history = history.GetRange(0,SAMPLESIZE);
                print(ex+ "\n" + average);
            }
            catch(Exception e)
            {
                Debug.Log($"format error ({e})\n{m}");
            }
        }
    }

    public Vector3 average
    {
        get
        {
            Vector3 t=Vector3.zero;
            for (var i = 0; i < SAMPLESIZE; i++) t += history[i].axis;
            var r = new Vector3(t.x / (float)SAMPLESIZE, 0f, t.z / (float)SAMPLESIZE);
            return new Vector3(
                (MINB > r.x || MINH < r.x)? r.x : 0f,
                (MINB > r.y || MINH < r.y)? r.y : 0f,
                (MINB > r.z || MINH < r.z)? r.z : 0f
                );
        }
    }
    [Range(0f, 5f)] [SerializeField]float asobi = 0.5f;
    float MINB => asobi * -1f;
    float MINH => asobi;
    /*
    public Vector3 average {  get {
            float dx = 0;
            float dy = 0;
            float dz = 0;
            for (var i = 0; i < SAMPLESIZE; i++)
            {
                dx += history[i].axis.x;
                dy += history[i].axis.y;
                dz += history[i].axis.z;
            }
            //return new Vector3(dx/(float)SAMPLESIZE, 0f, dz/ (float)SAMPLESIZE);
            //return new Vector3(-1f * dy / (float)SAMPLESIZE, dz / (float)SAMPLESIZE , dx / (float)SAMPLESIZE);
            return new Vector3(dy / (float)SAMPLESIZE, 0f , dx / (float)SAMPLESIZE);
        }
    }
    /**/
    [Range(1, 16)]
    public int SAMPLESIZE = 3;
    const int MAXHISTORY = 1024;
    const float MAXVALUE = 1024f;
    public List<DeviceContext> history = new List<DeviceContext>() { default, default, default, default };
    public DeviceContext deviceinput => history.Count>0? history[0]:default;
    public GameObject target;
}
[Serializable]
public struct DeviceContext
{
    public PressButton press;
    public int axisx, axisy, axisz;
    public int strength;
    public string gesture;
    public string[] gest_histry;
    const float ANGLE = -1f;
    const float ANGLEZ = -3f;
    public Vector3 axis { get { return new Vector3(((float)axisy / 1024f) * ANGLE, 0f, ((float)axisx / 1024f) * ANGLEZ); } }
    public bool isA { get { return press.HasFlag(PressButton.A); } }
    public bool isB { get { return press.HasFlag(PressButton.B); } }
    public bool isLogo { get { return press.HasFlag(PressButton.LOGO); } }

    public override string ToString()
    {
        return $"{press} ... A:{isA}, B:{isB}, Logo:{isLogo}, axis:{axis}, gesture:{string.Join(",",gest_histry)}";
    }
    [Flags]
    public enum PressButton
    {
        None = 0,
        A = 1<<0,
        B = 1<<1,
        LOGO = 1<<2
    }
}
