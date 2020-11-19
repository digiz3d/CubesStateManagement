using UnityEngine;
using UnityEngine.UI;
using RetardedNetworking;

public class GameMenuKeys : MonoBehaviour
{
    [SerializeField]
    private Text textServer;
    [SerializeField]
    private Text textClient;
    [SerializeField]
    private Text textHost;
    [SerializeField]
    private Text textClientId;

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

        textServer.text = "IsServer = " + (n.IsServer);
        textServer.color = n.IsServer ? Color.green : Color.red;
        textClient.text = "IsClient = " + (n.IsClient);
        textClient.color = n.IsClient ? Color.green : Color.red;
        textHost.text = "IsHost = " + (n.IsHost);
        textHost.color = n.IsHost ? Color.green : Color.red;

        textClientId.text = $"Client id = {Assets.Scripts.GameState.GameStateManager.Instance.gameState.currentPlayerId}";
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
