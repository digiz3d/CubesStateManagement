using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameState;

public class Puppet : MonoBehaviour
{
    int subscribedPlayerId = 0;

    // Update is called once per frame
    void Update()
    {
        PlayerState player;
        if (GameStateManager.GetState().playersById.TryGetValue(subscribedPlayerId, out player))
        {
            transform.position = player.position;
            transform.rotation = player.rotation;
        }
        else Destroy(gameObject);
    }

    public void SubscribeToPlayerId(int id)
    {
        subscribedPlayerId = id;
    }
}
