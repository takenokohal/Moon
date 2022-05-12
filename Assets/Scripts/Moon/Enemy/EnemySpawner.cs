using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Moon.Enemy
{
    public class EnemySpawner
    {
        private readonly EnemyController.Factory _factory;

        public EnemySpawner(EnemyController.Factory factory)
        {
            _factory = factory;
        }

        public UniTask<List<EnemyController>> Spawn(int stage)
        {
            return Stage1Spawner.Spawn(_factory, stage);
        }
    }
}