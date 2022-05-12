using Moon.Bullet;
using Moon.Common;
using UnityEngine;

namespace Moon.Shooter
{
    public class Cyclone : IShooter
    {
        private readonly Clock _clock;
        private readonly CircleRound _circleRound;
        private float _currentTheta;

        public Cyclone()
        {
            _clock = new Clock(1f / BulletManager.BulletPerSecond * 3f);
            _circleRound = new CircleRound(0.5f);
        }

        public void ShooterUpdate(
            IBulletManager bulletManager,
            Vector2 currentPosition,
            ShooterType shooterType,
            int level,
            bool isEnemy)
        {
            _circleRound.CountUp();
            if (!_clock.CountUp(level))
                return;

            for (int i = 0; i < 3; i++)
            {
                bulletManager.AddBullet(currentPosition,
                    new Circular(5, _circleRound.CurrentTheta + 2 * Mathf.PI / 3f * i).ToVector(),
                    shooterType,
                    isEnemy);
            }
        }
    }
}