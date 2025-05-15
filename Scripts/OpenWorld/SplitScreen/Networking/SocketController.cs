using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;


public class SocketController : MonoBehaviour
{

    Socket client;
    ManualResetEvent connectDone = new ManualResetEvent(false);

    const int BufferSize = 64;

    void Start()
    {
        StartClient();
    }

    void OnDestroy()
    {
        client.Shutdown(SocketShutdown.Both);
    }

    // ! Will freeze Start() function if not connected to server !
    void StartClient()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        try
        {
            client.BeginConnect(IPSelector.remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        client.Blocking = false; // default to be true, but we do !NOT! want blocking in Update()
    }

    void ConnectCallback(IAsyncResult ar)
    {
        try {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            Debug.LogFormat("Socket connected to {0}", client.RemoteEndPoint.ToString());
            connectDone.Set();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }

    public void SendSocket(string msg)
    {
        int bytes_sent = 0;
        try
        {
            Debug.LogFormat("bytes sent: {0}", msg);
            //bytes_sent = client.Send();
            byte[] data = Encoding.ASCII.GetBytes(msg);
            byte[] len_bytes = BitConverter.GetBytes(
                IPAddress.HostToNetworkOrder(data.Length));
            int sentlen = client.Send(len_bytes);
            Debug.Assert(sentlen == 4);
            sentlen = client.Send(data);
            Debug.Assert(sentlen == data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("bytes sent = " + bytes_sent.ToString() + "\n" + ex.ToString());
        }
    }
}
