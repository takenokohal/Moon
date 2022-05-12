using UnityEngine;

namespace Moon.Common
{
    public class Clock
    {
        public float Duration { get; }

        public float PerSec => 1f / Duration;

        private float _currentTime;


        public Clock(float duration)
        {
            Duration = duration;
            _currentTime = duration;
        }


        public bool CountUp(int i = 1)
        {
            var dt = Time.deltaTime;
            _currentTime += dt * i;

            var isOver = _currentTime >= Duration;

            if (isOver)
            {
                _currentTime -= Duration;
            }


            return isOver;
        }
    }
}