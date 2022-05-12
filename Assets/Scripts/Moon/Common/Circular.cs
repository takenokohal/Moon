using UnityEngine;

namespace Moon.Common
{
    public struct Circular
    {
        public float Radius { get; set; }
        public float Theta { get; set; }

        public Circular(float radius, float theta)
        {
            Radius = radius;
            Theta = theta;
        }

        public static Circular CreateFromVector2(Vector2 vector2)
        {
            return new Circular(vector2.magnitude, Mathf.Atan2(vector2.y, vector2.x));
        }

        public Vector2 ToVector()
        {
            return new Vector2(Mathf.Cos(Theta), Mathf.Sin(Theta)) * Radius;
        }
    }
}