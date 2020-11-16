using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ServerHandler
    {
        public static void ClientSaidThanks(Packet packet, Server server, Client client)
        {
            Log($"The client {packet.SenderClientId} said thanks.");
            GameStateManager.Instance.AddPlayer(packet.SenderClientId, Vector3.zero, Quaternion.identity);
        }

        private static void Log(string str)
        {
            Debug.Log($"[ServerHandler]: {str}");
        }
    }
}
