using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameState
{
    public class PlayerState
    {
        public readonly byte id;

        private List<TransformState> transformUpdates = new List<TransformState>();

        public class TransformState
        {
            public float ms;
            public Vector3 position;
            public Quaternion rotation;

            public TransformState(float time, Vector3 pos, Quaternion rot)
            {
                ms = time;
                position = pos;
                rotation = rot;
            }
        }

        public PlayerState(byte id, Vector3 position, Quaternion rotation)
        {
            this.id = id;
            transformUpdates.Add(new TransformState(Time.unscaledTime, position, rotation));
        }

        public void UpdateTransform(float time, Vector3 pos, Quaternion rot)
        {
            Debug.Log($"time = {time}");
            transformUpdates.Add(new TransformState(time, pos, rot));

            while (transformUpdates.Count > 3)
            {
                transformUpdates.RemoveAt(0);
            }
        }

        private TransformState GetLastTransformState()
        {
            if (transformUpdates.Count == 0)
                return new TransformState(0, Vector3.zero, Quaternion.identity);

            return transformUpdates[transformUpdates.Count - 1];
        }

        private TransformState GetPreviousTransformState()
        {
            if (transformUpdates.Count <= 1)
                return GetLastTransformState();

            return transformUpdates[transformUpdates.Count - 2];
        }

        public TransformState Interpolate(float factor)
        {
            TransformState lastTS = GetLastTransformState();
            TransformState previousTS = GetPreviousTransformState();

            float interpRatio = (factor - lastTS.ms) / 2f;
            Debug.Log($"{factor}-{lastTS.ms}={interpRatio}");

            float ms = Mathf.Lerp(previousTS.ms, lastTS.ms, interpRatio);
            Vector3 interPos = Vector3.Lerp(previousTS.position, lastTS.position, interpRatio);
            Quaternion interRot = Quaternion.Lerp(previousTS.rotation, lastTS.rotation, interpRatio);
            return new TransformState(ms, interPos, interRot);
        }
    }
}