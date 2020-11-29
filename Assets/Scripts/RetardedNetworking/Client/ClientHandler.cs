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
            GameStateManager.Instance.currentPlayerId = myId;
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
            Dictionary<byte, PlayerState> players = packet.ReadPlayersDictionary();
            foreach (KeyValuePair<byte, PlayerState> kvp in players)
            {
                if (kvp.Key == GameStateManager.Instance.currentPlayerId) continue;

                GameStateManager.Instance.gameState.UpdatePlayerPosition(kvp.Key, Time.unscaledTime, kvp.Value.GetLastTransformState().position, kvp.Value.GetLastTransformState().rotation);
            }
        }

        private static void Log(string str)
        {
            Debug.Log($"[ClientHandler]: {str}");
        }
    }
}