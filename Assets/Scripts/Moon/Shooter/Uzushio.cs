using Moon.Bullet;
using Moon.Common;
using UnityEngine;

namespace Moon.Shooter
{
    public class Uzushio : IShooter
    {
        private readonly Clock _clock;
        private readonly CircleRound _circleRound;
        private float _currentTheta;

        public Uzushio()
        {
            _clock = new Clock(1f / BulletManager.BulletPerSecond);
            _circleRound = new CircleRound(0.5f);
        }

        public void ShooterUpdate(IBulletManager bulletManager, Vector2 currentPosition, ShooterType shooterType,
            int level, bool isEnemy)
        {
            _circleRound.CountUp();
            if (!_clock.CountUp(level))
                return;

            bulletManager.AddBullet(currentPosition, new Circular(2f * level, _circleRound.CurrentTheta).ToVector(),
                shooterType,
                isEnemy);
        }
    }
}