using System;
using UnityEngine;

namespace Breach.AI
{
    public readonly struct NoiseEvent
    {
        public NoiseEvent(Vector3 position, float radius, float time, int sequence)
        {
            Position = position;
            Radius = radius;
            Time = time;
            Sequence = sequence;
        }

        public Vector3 Position { get; }
        public float Radius { get; }
        public float Time { get; }
        public int Sequence { get; }
    }

    public static class NoiseEventBus
    {
        public static event Action<NoiseEvent> NoiseRaised;
        private static NoiseEvent latestNoise;
        private static bool hasLatestNoise;
        private static int nextSequence;

        public static int LatestSequence => hasLatestNoise ? latestNoise.Sequence : 0;

        public static void Raise(Vector3 position, float radius)
        {
            var noise = new NoiseEvent(position, radius, Time.time, ++nextSequence);
            latestNoise = noise;
            hasLatestNoise = true;
            NoiseRaised?.Invoke(noise);
        }

        public static bool TryGetLatestNoise(out NoiseEvent noise)
        {
            noise = latestNoise;
            return hasLatestNoise;
        }
    }
}
