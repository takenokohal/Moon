using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace Moon.Bullet
{
    [DefaultExecutionOrder(100)]
    public class BulletManager : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private readonly List<BulletData> _originBulletList = new(100000);
        public List<BulletData> OriginBulletList => _originBulletList;

        public void AddBullet(BulletData bulletData)
        {
            bulletData.Lifetime = 5;
            _originBulletList.Add(bulletData);
        }


        private NativeArray<BulletData> _tempDataArray;

        private JobHandle _jobHandle;

        private Transform _camera;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            var emit = _originBulletList.Count - _particleSystem.particleCount;
            var deltaTime = Time.smoothDeltaTime;
            if (emit > 0)
                _particleSystem.Emit(emit);


            _tempDataArray = new NativeArray<BulletData>(_originBulletList.Count, Allocator.TempJob);
            for (int i = 0; i < _tempDataArray.Length; i++)
            {
                _tempDataArray[i] = _originBulletList[i];
            }


            _jobHandle = new ParticleJob
            {
                BulletDataArray = _tempDataArray,
                DeltaTime = deltaTime,
                CameraPosition = _camera.position
            }.Schedule(_particleSystem, 32);
            JobHandle.ScheduleBatchedJobs();
        }

        private void LateUpdate()
        {
            _jobHandle.Complete();
            _originBulletList.Clear();
            for (int i = 0; i < _tempDataArray.Length; i++)
            {
                if (!_tempDataArray[i].IsKilled)
                    _originBulletList.Add(_tempDataArray[i]);
            }

            _tempDataArray.Dispose(_jobHandle);
        }


        public struct BulletData
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float ColorHue;
            public float Lifetime;

            public bool IsEnemy;

            public bool IsKilled;
        }

        [BurstCompile]
        private struct ParticleJob : IJobParticleSystemParallelFor
        {
            public NativeArray<BulletData> BulletDataArray;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public Vector2 CameraPosition;

            public void Execute(ParticleSystemJobData jobData, int index)
            {
                if (index >= BulletDataArray.Length)
                {
                    var life = jobData.aliveTimePercent;
                    life[index] = 100f;
                    return;
                }

                var data = BulletDataArray[index];
                var nextPosition = data.Position + data.Velocity * DeltaTime;

                var dist = Vector2.Distance(CameraPosition, nextPosition);

                if (dist > 10)
                {
                    Kill(jobData, index);
                    return;
                }

                var lifetime = data.Lifetime - DeltaTime;
                if (lifetime <= 0)
                {
                    Kill(jobData, index);
                    return;
                }

                data.Position = nextPosition;
                data.Lifetime = lifetime;
                BulletDataArray[index] = data;

                var positions = jobData.positions;
                positions[index] = nextPosition;

                var colors = jobData.startColors;
                colors[index] = Color.HSVToRGB(data.ColorHue, 1, 1);

                var sizes = jobData.sizes;
                sizes.x[index] = lifetime / (data.IsEnemy ? 10 : 20);
            }


            private void Kill(ParticleSystemJobData jobData, int index)
            {
                BulletDataArray[index] = new BulletData
                {
                    IsKilled = true
                };

                var life = jobData.aliveTimePercent;
                life[index] = 100f;
            }
        }
    }
}