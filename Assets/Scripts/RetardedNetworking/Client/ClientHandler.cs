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
            Packet thanks = new Packet(PacketType.THANKS);
            thanks.Write(myId);
            client.SendPacketToServer(thanks);
        }

        public static void GetGameState(Packet packet, Server server, Client client)
        {
            GameStateManager.SetGameState(packet.ReadGameState());
        }

        public static void ClientMoved(Packet packet, Server server, Client client)
        {
            GameStateManager.UpsertPlayer(packet.ReadByte(), packet.ReadVector3(), packet.ReadQuaternion());
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}