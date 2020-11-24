using UnityEngine;
using Assets.Scripts.GameState;

public class Puppet : MonoBehaviour
{
    byte subscribedPlayerId = 0;

    void Update()
    {
        PlayerState player;
        if (GameStateManager.Instance.gameState.players.TryGetValue(subscribedPlayerId, out player))
        {
            PlayerState.TransformState interpolated = player.Interpolate(Time.unscaledTime);
            transform.position = interpolated.position;
            transform.rotation = interpolated.rotation;
        }
        else Destroy(gameObject);
    }

    public void SubscribeToPlayerId(byte id)
    {
        subscribedPlayerId = id;
    }
}
