using System;
using UnityEngine;

namespace Breach.AI
{
    public readonly struct NoiseEvent
    {
        public NoiseEvent(Vector3 position, float radius, float time)
        {
            Position = position;
            Radius = radius;
            Time = time;
        }

        public Vector3 Position { get; }
        public float Radius { get; }
        public float Time { get; }
    }

    public static class NoiseEventBus
    {
        public static event Action<NoiseEvent> NoiseRaised;

        public static void Raise(Vector3 position, float radius)
        {
            NoiseRaised?.Invoke(new NoiseEvent(position, radius, Time.time));
        }
    }
}
