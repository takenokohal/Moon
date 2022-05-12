using System.Collections.Generic;
using Moon.Shooter;
using Unity.Collections;
using UnityEngine;

namespace Moon.Bullet
{
    public interface IBulletManager
    {
        public NativeArray<BulletData> BulletDataArray { get; }

        public void RegisterShooterHolder(ShooterHolder shooter);
        public void RegisterDamageable(IDamageable damageable);
        public void UnRegisterShooterHolder(ShooterHolder shooter);
        public void UnRegisterDamageable(IDamageable damageable);

        public void AddBullet(Vector2 position, Vector2 velocity, ShooterType shooterType, bool isEnemy);

        public void StopShooting();

        public void StartShooting();
    }
}