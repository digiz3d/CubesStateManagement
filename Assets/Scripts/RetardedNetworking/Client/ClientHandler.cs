using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ClientHandler
    {
        public static void GetMyClientId(Packet packet, Server server, Client client)
        {
            byte myId = packet.ReadByte();
            client.Id = myId;
            GameStateManager.SetCurrentPlayerId(myId);
            Log($"The server send me my id = {myId}");
            Packet thanks = new Packet(PacketType.THANKS);
            client.SendPacketToServer(thanks);
        }

        public static void GetGameState(Packet packet, Server server, Client client)
        {
            GameStateManager.SetGameState(packet.ReadGameState());
        }

        public static void ClientMoved(Packet packet, Server server, Client client)
        {
            var gameState = GameStateManager.Instance.gameState;
            var playerId = packet.ReadByte();
            var position = packet.ReadVector3();
            var rotation = packet.ReadQuaternion();
            gameState.players[playerId].position = position;
            gameState.players[playerId].rotation = rotation;
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}