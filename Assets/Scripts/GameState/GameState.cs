using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.GameState
{
    public class GameState
    {
        public byte currentPlayerId = 0;
        public Dictionary<byte, PlayerState> players;
        public string serverName = "My server";

        public GameState()
        {
            players = new Dictionary<byte, PlayerState>();
        }

        public void SetCurrentPlayerId(byte id)
        {
            currentPlayerId = id;
        }

        public void UpsertPlayer(byte id, Vector3 position, Quaternion rotation)
        {
            if (players.ContainsKey(id))
            {
                players[id].position = position;
                players[id].rotation = rotation;
            }
            else
            {
                PlayerState p = new PlayerState(id, position, rotation);
                players.Add(id, p);
            }
        }
    }
}