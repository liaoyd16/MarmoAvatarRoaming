using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IPSelector : MonoBehaviour
{
    [SerializeField] string default_server_ip = "127.0.0.1";
    public static IPEndPoint remoteEP;
    TMPro.TMP_InputField ip_input, port_input;
    [SerializeField] int loaded_scene;
    // Start is called before the first frame update
    void Start()
    {
        ip_input = transform.Find("IP").GetComponent<TMPro.TMP_InputField>();
        port_input = transform.Find("Port").GetComponent<TMPro.TMP_InputField>();
    }

    // Update is called once per frame
    public void onConfirmClicked()
    {
        int server_port = 11000;
        int.TryParse(port_input.text, out server_port);

        try {
            remoteEP = new IPEndPoint(IPAddress.Parse(ip_input.text), server_port);
        } catch {
            remoteEP = new IPEndPoint(IPAddress.Parse(default_server_ip), server_port);
        }
        
        SceneManager.LoadScene(loaded_scene);
    }
}
