using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ServerHandler
    {
        public static void ClientSaidThanks(Packet packet, Server server, Client client)
        {
            byte clientConfirmedId = packet.ReadByte();
            if (clientConfirmedId != packet.SenderClientId) Debug.Log($"The client {packet.SenderClientId} is lying about his identity !");
            Log($"The client {clientConfirmedId} said thanks.");
            GameStateManager.Instance.gameState.AddPlayer(packet.SenderClientId, Vector3.zero, Quaternion.identity);
            Packet gameInfo = new Packet(PacketType.GIVE_CLIENT_GAME_STATE);
            GameStateManager.Instance.gameState.currentPlayerId = packet.SenderClientId;
            gameInfo.WriteGameState(GameStateManager.Instance.gameState);
            server.SendPacketToAllClients(gameInfo);
            GameStateManager.Instance.gameState.currentPlayerId = ClientIdsManager.SERVER_CLIENT_ID;
        }

        public static void ClientTransform(Packet packet, Server server, Client client)
        {
            var clientId = packet.SenderClientId;
            var position = packet.ReadVector3();
            var rotation = packet.ReadQuaternion();

            GameStateManager.Instance.gameState.UpdatePlayerPosition(clientId, Time.unscaledTime, position, rotation);
        }

        private static void Log(string str)
        {
            Debug.Log($"[ServerHandler]: {str}");
        }
    }
}
