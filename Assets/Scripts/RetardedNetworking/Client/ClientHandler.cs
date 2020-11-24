using Assets.Scripts.GameState;
using UnityEngine;
using System.Collections.Generic;

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

        public static void ClientsTransforms(Packet packet, Server server, Client client)
        {
            float serverTime = packet.ReadFloat();
            Dictionary<byte, PlayerState> players = packet.ReadPlayersDictionary();
            foreach (KeyValuePair<byte, PlayerState> kvp in players)
            {
                if (kvp.Key == GameStateManager.Instance.gameState.currentPlayerId) continue;

                GameStateManager.Instance.gameState.UpdatePlayerPosition(kvp.Key, serverTime, kvp.Value.Interpolate(1f).position, kvp.Value.Interpolate(1f).rotation);
            }
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}