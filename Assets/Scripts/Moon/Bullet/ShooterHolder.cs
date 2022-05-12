using System;
using System.Collections.Generic;
using System.Linq;
using Moon.Shooter;
using UniRx;
using UnityEngine;
using VContainer;

namespace Moon.Bullet
{
    public class ShooterHolder
    {
        private readonly Func<Vector2> _position;

        public ShooterHolder(Func<Vector2> position, bool isEnemy)
        {
            _position = position;
            _isEnemy = isEnemy;
        }

        private readonly Dictionary<ShooterType, IShooter> _shooters = new();
        private readonly ReactiveDictionary<ShooterType, int> _shooterLevels = new();
        public IReadOnlyReactiveDictionary<ShooterType, int> ShooterLevels => _shooterLevels;

        private readonly bool _isEnemy;

        public bool TryGet(ShooterType shooterType, out int level)
        {
            var b = _shooterLevels.TryGetValue(shooterType, out var v);
            level = b ? v : 0;
            return b;
        }

        public void AddShooter(ShooterType shooterType)
        {
            if (_shooterLevels.ContainsKey(shooterType))
            {
                _shooterLevels[shooterType]++;
            }
            else
            {
                if (shooterType != ShooterType.Moon)
                    _shooters.Add(shooterType, ShooterCreator.Create(shooterType));
                _shooterLevels.Add(shooterType, 1);
            }
        }

        public void ShooterUpdate(IBulletManager bulletManager)
        {
            foreach (var (key, data) in _shooters)
            {
                data.ShooterUpdate(bulletManager, _position(), key, _shooterLevels[key], _isEnemy);
            }
        }
    }
}