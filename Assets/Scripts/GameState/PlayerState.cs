using UnityEngine;

namespace Assets.Scripts.GameState
{
    public class PlayerState
    {
        public readonly int id;
        public Vector3 position;
        public Quaternion rotation;

        public PlayerState(int id, Vector3 position, Quaternion rotation)
        {
            this.id = id;
            this.position = position;
            this.rotation = rotation;
        }
    }
}