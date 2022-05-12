using UnityEngine;

namespace Moon.Bullet
{
    public struct BulletData
    {
        public Vector2 Position;
        public float ElapsedTime;

        public Vector2 Velocity { get; }
        public Color Color { get; }

        public bool IsEnemy { get; }

        public bool IsActive;


        public BulletData(Vector2 startPosition, Vector2 velocity, Color color, bool isEnemy)
        {
            Position = startPosition;
            ElapsedTime = 0;

            Velocity = velocity;
            Color = color;

            IsEnemy = isEnemy;

            IsActive = true;
        }
    }
}