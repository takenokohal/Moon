using Moon.Bullet;
using Moon.Common;
using UnityEngine;

namespace Moon.Shooter
{
    public class Yamaarashi : IShooter
    {
        private readonly Clock _clock;

        public Yamaarashi()
        {
            _clock = new Clock(1f);
        }

        public void ShooterUpdate(IBulletManager bulletManager, Vector2 currentPosition, ShooterType shooterType,
            int level, bool isEnemy)
        {
            if (!_clock.CountUp(level))
                return;

            var vv = BulletManager.BulletPerSecond * _clock.Duration;
            for (int i = 0; i < vv; i++)
            {
                var v = new Circular(2 * level, Mathf.PI * 2f / vv * i);
                bulletManager.AddBullet(currentPosition, v.ToVector(), shooterType, isEnemy);
            }
        }
    }
}