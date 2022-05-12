using UnityEngine;

namespace Moon.Bullet
{
    public interface IDamageable
    {
        public Vector2 GetPosition();
        public void OnDamaged(int damage);

        public bool IsEnemy { get; }
    }
}