using Moon.Bullet;
using Moon.Common;
using UnityEngine;

namespace Moon.Shooter
{
    public class Sakura : IShooter
    {
        private readonly Clock _clock;
        private readonly CircleRound _circleRound;

        public Sakura()
        {
            _clock = new Clock(5f * 2f / BulletManager.BulletPerSecond);
            _circleRound = new CircleRound(1f);
        }

        public void ShooterUpdate(IBulletManager bulletManager, Vector2 currentPosition, ShooterType shooterType,
            int level, bool isEnemy)
        {
            _circleRound.CountUp();

            if (!_clock.CountUp(level))
                return;


            for (int i = 0; i < 5; i++)
            {
                var v = new Circular(2 * level, _circleRound.CurrentTheta * level + Mathf.PI * 2f / 5f * i);
                bulletManager.AddBullet(currentPosition, v.ToVector(), shooterType, isEnemy);
            }

            for (int i = 0; i < 5; i++)
            {
                var v = new Circular(2 * level, -(_circleRound.CurrentTheta * level + Mathf.PI * 2f / 5f * i));
                bulletManager.AddBullet(currentPosition, v.ToVector(), shooterType, isEnemy);
            }
        }
    }
}