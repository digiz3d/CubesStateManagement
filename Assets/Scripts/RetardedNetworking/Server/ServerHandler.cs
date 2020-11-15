using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ServerHandler
    {
        public static void ClientSaidThanks(Packet pck, Server server, Client client)
        {
            Log($"The client {pck.SenderClientId} said thanks.");
            GameStateManager.Instance.AddPlayer(client.Id, Vector3.zero, Quaternion.identity);
        }

        private static void Log(string str)
        {
            Debug.Log($"[ServerHandler]: {str}");
        }
    }
}
