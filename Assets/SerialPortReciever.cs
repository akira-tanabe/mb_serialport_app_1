using System;
using System.Threading;
using System.IO.Ports;
using UnityEngine;
/// <summary>
/// SerialPortインスタンスを作成して機器と接続する
/// </summary>
[Serializable]
public class SerialPortReciever
{
    [SerializeField] SerialPort _serial;
    Thread _thread;
    public bool enable;
    public string recieve;
    public Action<string> onreceiveaction;
    /// <summary>
    /// SerialPortインスタンスの作成
    /// </summary>
    /// <param name="portName"></param>
    public static SerialPortReciever Create(string portName)
    {
        var r = new SerialPortReciever();
        if (r._Create(portName))
        {
            r._thread = new Thread(r.RecieverAct);
            r._thread.Start();
            return r;
        }
        else
        {
            Debug.LogError("create failure");
            return null;
        }
    }

    bool _Create(string portName)
    {
        _serial = new SerialPort();
        _serial.BaudRate = 115200;
        _serial.Parity   = Parity.None;
        _serial.DataBits = 8;
        _serial.StopBits = StopBits.One;
        _serial.Handshake= Handshake.None;
        _serial.PortName = portName;
        try
        {
            _serial.Open();
            _serial.DtrEnable = true;
            _serial.RtsEnable = true;
            _serial.DiscardInBuffer();
            _serial.ReadTimeout = 5000;
            this.enable = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"failure.. {e}");
            _serial.Close();
            _serial = null;
            return false;
        }
    }

    void RecieverAct()
    {
        while (enable)
        {
            try
            {
                recieve = _serial?.ReadLine();
                onreceiveaction?.Invoke(recieve);
            }
            catch (Exception e)
            {
                recieve = e.ToString();
            }
        }
    }

    public void Dispose()
    {
        enable = false;
        _thread?.Abort();
        _thread = null;
        _serial?.Close();
        _serial = null;
    }
}