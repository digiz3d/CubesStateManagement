using UnityEngine;

namespace RetardedNetworking
{
    public static class ClientHandler
    {
        public static void GetMyClientId(Packet pck, Server server, Client client)
        {
            client.Id = pck.ReadByte();
            Log($"The server send me my id = {client.Id}");
            Packet thanks = new Packet(PacketType.THANKS, client.Id);
            client.SendPacketToServer(thanks);
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}