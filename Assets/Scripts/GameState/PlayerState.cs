using System.Collections.Generic;
using UnityEngine;
using RetardedNetworking;

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
            transformUpdates.Add(new TransformState(time, pos, rot));

            while (transformUpdates.Count > 3)
            {
                transformUpdates.RemoveAt(0);
            }
        }

        public TransformState GetLastTransformState()
        {
            if (transformUpdates.Count == 0)
                return new TransformState(0, Vector3.zero, Quaternion.identity);

            return transformUpdates[transformUpdates.Count - 1];
        }

        private bool GetTransformToLerp(float renderingTime, out TransformState first, out TransformState second)
        {
            first = transformUpdates.FindLast(t => t.ms <= renderingTime);
            second = transformUpdates.Find(t => t.ms > renderingTime);
            if (first == null || second == null) return false;
            return true;
        }

        public TransformState Interpolate(float currentTime)
        {
            float renderingTime = currentTime - (2 * (1f / NetworkManager.tickrate));
            if (!GetTransformToLerp(renderingTime, out TransformState firstTs, out TransformState lastTs))
                return GetLastTransformState();

            float interpRatio = (renderingTime - firstTs.ms) / (lastTs.ms - firstTs.ms);
            //Debug.Log("(currentTime - firstTs.ms) / (lastTs.ms - firstTs.ms) = interpRatio");
            //Debug.Log($" ({renderingTime} - {firstTs.ms}) / ({lastTs.ms} - {firstTs.ms}) = {interpRatio}");

            float ms = Mathf.Lerp(firstTs.ms, lastTs.ms, interpRatio);
            Vector3 interPos = Vector3.Lerp(firstTs.position, lastTs.position, interpRatio);
            Quaternion interRot = Quaternion.Lerp(firstTs.rotation, lastTs.rotation, interpRatio);
            return new TransformState(ms, interPos, interRot);
        }
    }
}