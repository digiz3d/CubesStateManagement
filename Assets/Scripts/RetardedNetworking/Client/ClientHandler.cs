using Assets.Scripts.GameState;
using UnityEngine;

namespace RetardedNetworking
{
    public static class ClientHandler
    {
        public static void GetMyClientId(Packet packet, Server server, Client client)
        {
            client.Id = packet.ReadByte();
            Log($"The server send me my id = {client.Id}");
            Packet thanks = new Packet(PacketType.THANKS);
            client.SendPacketToServer(thanks);
        }

        public static void GetGameState(Packet packet, Server server, Client client)
        {
            GameStateManager.SetGameState(packet.ReadGameState());
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}