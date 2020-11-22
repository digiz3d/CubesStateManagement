﻿using UnityEngine;
using Assets.Scripts.GameState;

public class Puppet : MonoBehaviour
{
    byte subscribedPlayerId = 0;

    void Update()
    {
        PlayerState player;
        if (GameStateManager.Instance.gameState.players.TryGetValue(subscribedPlayerId, out player))
        {
            transform.position = player.position;
            transform.rotation = player.rotation;
        }
        else Destroy(gameObject);
    }

    public void SubscribeToPlayerId(byte id)
    {
        subscribedPlayerId = id;
    }
}
