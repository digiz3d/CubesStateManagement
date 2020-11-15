using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.GameState
{
    public class GameState
    {
        public int currentPlayerId = 0;
        public List<PlayerState> players;
        public Dictionary<int, PlayerState> playersById;
        public string serverName = "My server";

        public GameState()
        {
            players = new List<PlayerState>();
            playersById = new Dictionary<int, PlayerState>();
        }

        public void SetCurrentPlayerId(int id)
        {
            currentPlayerId = id;
        }

        public void AddPlayer(int id, Vector3 position, Quaternion rotation)
        {
            PlayerState p = new PlayerState(id, position, rotation);
            players.Add(p);
            playersById.Add(id, p);
        }
    }
}