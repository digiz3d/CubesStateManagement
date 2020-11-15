using UnityEngine;
using UnityEngine.UI;
using RetardedNetworking;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Text textServer;
    [SerializeField]
    private Text textClient;
    [SerializeField]
    private Text textHost;

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.S))
            ToggleServer();

        if (Input.GetKeyUp(KeyCode.C))
            ToggleClient();

        if (Input.GetKeyUp(KeyCode.H))
            ToggleHost();

        NetworkManager n = NetworkManager.Instance;
        if (n == null) return;

        Debug.Log("every time");

        string res = "";

        if (textServer == null)
        {
            res += "textServer null & ";
        }
        if (textClient == null)
        {
            res += "textClient null & ";
        }
        if (textHost == null)
        {
            res += "textHost null & ";
        }

        if (res != "")
        {
            Debug.Log(res);
            return;
        }

        textServer.text = "IsServer = " + (n.IsServer);
        textServer.color = n.IsServer ? Color.green : Color.red;
        textClient.text = "IsClient = " + (n.IsClient);
        textClient.color = n.IsClient ? Color.green : Color.red;
        textHost.text = "IsHost = " + (n.IsServer && n.IsClient);
        textHost.color = n.IsHost ? Color.green : Color.red;
    }

    private void ToggleServer()
    {
        NetworkManager n = NetworkManager.Instance;
        if (n == null) return;

        if (n.IsServer)
            n.StopServer();
        else
            n.StartServer();
    }

    private void ToggleClient()
    {
        NetworkManager n = NetworkManager.Instance;
        if (n == null) return;

        if (n.IsClient)
            n.StopClient();
        else
            n.StartClient();
    }

    public void ToggleHost()
    {
        NetworkManager n = NetworkManager.Instance;
        if (n == null) return;

        if (n.IsHost)
            n.StopHost();
        else
            n.StartHost();
    }
}
