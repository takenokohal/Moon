using Moon.Bullet;
using Moon.Common;
using UnityEngine;

namespace Moon.Shooter
{
    public class Momiji : IShooter
    {
        private readonly Clock _clock;
        private readonly CircleRound _circleRound;

        private const int Count = 7;

        public Momiji()
        {
            _clock = new Clock(Count * 2f / BulletManager.BulletPerSecond);
            _circleRound = new CircleRound(2f);
        }

        public void ShooterUpdate(IBulletManager bulletManager, Vector2 currentPosition, ShooterType shooterType,
            int level, bool isEnemy)
        {
            _circleRound.CountUp();

            if (!_clock.CountUp(level))
                return;
            

            for (int i = 0; i < Count; i++)
            {
                var v = new Circular(5 * level, _circleRound.CurrentTheta * level + Mathf.PI * 2f / Count * i);
                bulletManager.AddBullet(currentPosition, v.ToVector(), shooterType, isEnemy);
            }

            for (int i = 0; i < Count; i++)
            {
                var v = new Circular(5 * level, -(_circleRound.CurrentTheta * level + Mathf.PI * 2f / Count * i));
                bulletManager.AddBullet(currentPosition, v.ToVector(), shooterType, isEnemy);
            }
        }
    }
}