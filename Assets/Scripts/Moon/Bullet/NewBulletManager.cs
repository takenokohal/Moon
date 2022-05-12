/*using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Moon.Bullet
{
    public class NewBulletManager : MonoBehaviour, IBulletManager
    {
        private BulletView _bulletView;

        private NativeArray<BulletData> _bulletArray;
        public NativeArray<BulletData> BulletDataList => _bulletArray; 

        private JobHandle _jobHandle;

        [SerializeField] private Material material;

        private readonly List<ShooterHolder> _shooters = new();

        public void RegisterShooterHolder(ShooterHolder shooter) => _shooters.Add(shooter);

        private void Start()
        {
            _bulletView = new BulletView(material);

            _bulletArray = new NativeArray<BulletData>(1000000, Allocator.Persistent);
        }

        private void OnDisable()
        {
            _bulletArray.Dispose(_jobHandle);
            _bulletView.Dispose();
        }

        private void Update()
        {
            _jobHandle.Complete();

            _bulletView.UpdateView(BulletDataList);

            foreach (var shooter in _shooters)
            {
                shooter.ShooterUpdate(this);
            }

            _jobHandle = new BulletJob
            {
                BulletDataArray = _bulletArray,
                DeltaTime = Time.deltaTime,
            }.Schedule(_bulletArray.Length, 16, _jobHandle);

            JobHandle.ScheduleBatchedJobs();
        }


        public void AddBullet(Vector2 position, Vector2 velocity, Color color)
        {
            var v = new BulletData(position, velocity, color, false);
            for (int i = 0; i < _bulletArray.Length; i++)
            {
                if (!_bulletArray[i].IsActive)
                {
                    _bulletArray[i] = v;
                    return;
                }
            }
        }


        [BurstCompile]
        private struct BulletJob : IJobParallelFor
        {
            public NativeArray<BulletData> BulletDataArray;
            [ReadOnly] public float DeltaTime;
            private const float MaxTime = 60;

            public void Execute(int index)
            {
                var data = BulletDataArray[index];
                if (!data.IsActive)
                    return;

                var et = data.ElapsedTime + DeltaTime;
                if (et > MaxTime)
                {
                    data.IsActive = false;
                    return;
                }

                data.Position += data.Velocity * DeltaTime;
                data.ElapsedTime += et;
                BulletDataArray[index] = data;
            }
        }
    }
}*/