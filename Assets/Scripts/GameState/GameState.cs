using UnityEngine;
using System;
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

        public void UpdatePlayerPosition(byte id, float time, Vector3 position, Quaternion rotation)
        {
            if (!players.ContainsKey(id)) throw new Exception("This player id doesn't exists.");

            players[id].UpdateTransform(time, position, rotation);
        }

        public void AddPlayer(byte id, Vector3 position, Quaternion rotation)
        {
            if (players.ContainsKey(id)) throw new Exception("This player id already exists.");

            PlayerState p = new PlayerState(id, position, rotation);
            players.Add(id, p);
        }
    }
}