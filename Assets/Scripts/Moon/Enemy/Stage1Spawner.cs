using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Moon.Bullet;
using Moon.Common;
using Moon.Shooter;
using UnityEngine;

namespace Moon.Enemy
{
    public static class Stage1Spawner
    {
        public static async UniTask<List<EnemyController>> Spawn(EnemyController.Factory factory, int stageLevel)
        {
            var list = new List<EnemyController>();
            var r = Random.Range(10f, 15f);


            var length = 3 + stageLevel / 3;
            for (int i = 0; i < length; i++)
            {
                var e = factory.Create(2 + stageLevel / 5);

                for (int j = 0; j < 1 + stageLevel / 2; j++)
                {
                    var addType = EnumExtension.GetRandomValue<ShooterType>(1, 1);
                    e.AddShooter(addType);
                }

                var t = e.transform;
                var theta = 2f * Mathf.PI * i / length;
                theta += 2f * Mathf.PI / 4f;
                t.position = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)) * r;

                list.Add(e);

                await UniTask.Delay(200);
            }

            return list;
        }
    }
}