using UnityEngine;
using Assets.Scripts.GameState;

public class PlayerInput : MonoBehaviour
{
    readonly float speed = 0.01f;

    byte attachedToPlayerId;
    const float tickrate = 2f;
    float timeElapsedSinceLastTick = 0;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        gameObject.transform.Translate(x * speed, 0, y * speed);

        timeElapsedSinceLastTick += Time.unscaledDeltaTime;

        if (attachedToPlayerId != 0 && timeElapsedSinceLastTick >= (1f / tickrate))
        {
            timeElapsedSinceLastTick = 0; // maybe (1f/tickrate) - timeElapsedSinceLastTick;
            Debug.Log($"x = {x}, y = {y}");
            GameStateManager.Move(gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    public void AttachToPlayer(byte id)
    {
        attachedToPlayerId = id;
    }
}
