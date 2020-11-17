using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ServerHandler
    {
        public static void ClientSaidThanks(Packet packet, Server server, Client client)
        {
            Log($"The client {packet.SenderClientId} said thanks.");
            GameStateManager.UpsertPlayer(packet.SenderClientId, Vector3.zero, Quaternion.identity);
            Packet gameInfo = new Packet(PacketType.GIVE_CLIENT_GAME_STATE);
            gameInfo.Write(GameStateManager.Instance.gameState);
            server.SendPacketToAllClients(gameInfo);
        }

        private static void Log(string str)
        {
            Debug.Log($"[ServerHandler]: {str}");
        }
    }
}
