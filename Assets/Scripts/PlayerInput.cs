using UnityEngine;
using Assets.Scripts.GameState;

public class PlayerInput : MonoBehaviour
{
    byte attachedToPlayerId;
    const float tickrate = 10f;
    float timeElapsedSinceLastTick = 0;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        gameObject.transform.Translate(x, 0, y);

        timeElapsedSinceLastTick += Time.deltaTime;

        if (attachedToPlayerId != 0 && timeElapsedSinceLastTick >= (1f/tickrate))
        {
            timeElapsedSinceLastTick = 0;
            Debug.Log($"x = {x}, y = {y}");
            GameStateManager.Move(gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    public void AttachToPlayer(byte id)
    {
        attachedToPlayerId = id;
    }
}
