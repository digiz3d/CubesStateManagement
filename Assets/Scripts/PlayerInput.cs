using UnityEngine;
using Assets.Scripts.GameState;

public class PlayerInput : MonoBehaviour
{
    readonly float speed = 0.01f;

    byte attachedToPlayerId;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        gameObject.transform.Translate(x * speed, 0, y * speed);

        GameStateManager.Move(gameObject.transform.position, gameObject.transform.rotation);
    }

    public void AttachToPlayer(byte id)
    {
        attachedToPlayerId = id;
    }
}
