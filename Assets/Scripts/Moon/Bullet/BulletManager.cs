using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public void AddBullet(BulletData bulletData) => _originBulletList.Add(bulletData);

        private NativeArray<BulletData> _tempDataArray;

        private JobHandle _jobHandle;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            var emit = _originBulletList.Count - _particleSystem.particleCount;
            var deltaTime = Time.smoothDeltaTime;
            if (emit > 0)
                _particleSystem.Emit(new ParticleSystem.EmitParams()
                {
                    startSize = 0.1f
                }, emit);

            _tempDataArray = new NativeArray<BulletData>(_originBulletList.Count, Allocator.TempJob);
            for (int i = 0; i < _tempDataArray.Length; i++)
            {
                _tempDataArray[i] = _originBulletList[i];
            }

            _jobHandle = new ParticleJob
            {
                BulletDataArray = _tempDataArray,
                DeltaTime = deltaTime
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

            public bool IsKilled;
        }

        [BurstCompile]
        private struct ParticleJob : IJobParticleSystemParallelFor
        {
            public NativeArray<BulletData> BulletDataArray;
            [ReadOnly] public float DeltaTime;

            public void Execute(ParticleSystemJobData jobData, int index)
            {
                var data = BulletDataArray[index];
                var nextPosition = data.Position + data.Velocity * DeltaTime;

                if (nextPosition.sqrMagnitude < 100)
                {
                    data.Position = nextPosition;
                    BulletDataArray[index] = data;

                    var positions = jobData.positions;
                    positions[index] = nextPosition;

                    var colors = jobData.startColors;
                    colors[index] = Color.HSVToRGB(data.ColorHue, 1, 1);
                }
                //Kill
                else
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
}