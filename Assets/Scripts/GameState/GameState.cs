using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.GameState
{
    public class GameState
    {
        public int currentPlayerId = 0;
        public Dictionary<int, PlayerState> players;
        public string serverName = "My server";

        public GameState()
        {
            players = new Dictionary<int, PlayerState>();
        }

        public void SetCurrentPlayerId(int id)
        {
            currentPlayerId = id;
        }

        public void UpsertPlayer(int id, Vector3 position, Quaternion rotation)
        {
            PlayerState p = new PlayerState(id, position, rotation);
            players.Add(id, p);
        }
    }
}