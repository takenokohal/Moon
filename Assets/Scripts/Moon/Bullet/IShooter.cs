using Moon.Shooter;
using UnityEngine;

namespace Moon.Bullet
{
    public interface IShooter
    {
        public void ShooterUpdate(IBulletManager bulletManager, Vector2 currentPosition, ShooterType shooterType,
            int level , bool isEnemy);
    }
}