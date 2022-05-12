using UnityEngine;

namespace Moon.Common
{
    public class CircleRound
    {
        public CircleRound(float lapTime)
        {
            LapTime = lapTime;
        }

        public float LapTime { get; }
        public float RotationPerSecond => 1f / LapTime;

        public float CurrentTheta { get; private set; }

        public bool CountUp()
        {
            CurrentTheta += Time.deltaTime * RotationPerSecond;
            var isOver = CurrentTheta > 2 * Mathf.PI;
            if (isOver)
                CurrentTheta -= 2 * Mathf.PI;

            return isOver;
        }
    }
}