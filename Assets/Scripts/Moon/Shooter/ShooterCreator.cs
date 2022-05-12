using System;
using Moon.Bullet;

namespace Moon.Shooter
{
    public static class ShooterCreator
    {
        public static IShooter Create(ShooterType shooterType)
        {
            return shooterType switch
            {
                ShooterType.Cyclone => new Cyclone(),
                ShooterType.Sakura => new Sakura(),
                ShooterType.Yamaarashi => new Yamaarashi(),
                ShooterType.None => null,
                ShooterType.Momiji => new Momiji(),
                ShooterType.Uzushio => new Uzushio(),
                ShooterType.Moon => null,
                _ => throw new ArgumentOutOfRangeException(nameof(shooterType), shooterType, null)
            };
        }
    }
}