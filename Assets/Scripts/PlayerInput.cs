using UnityEngine;
using Assets.Scripts.GameState;

public class PlayerInput : MonoBehaviour
{
    readonly float speed = 0.01f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        gameObject.transform.Translate(x * speed, 0, y * speed);

        byte playerId = GameStateManager.Instance.currentPlayerId;
        GameStateManager.Instance.gameState.players[playerId].UpdateTransform(Time.unscaledTime, gameObject.transform.position, gameObject.transform.rotation);
    }
}
