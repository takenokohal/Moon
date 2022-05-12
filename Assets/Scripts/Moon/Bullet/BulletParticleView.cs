using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace Moon.Bullet
{
    public class BulletParticleView : IDisposable
    {
        private readonly ParticleSystem _particleSystem;
        
        public BulletParticleView(ParticleSystem particleSystem)
        {
            _particleSystem = particleSystem;
        }

        public void Clear()
        {
            _particleSystem.Clear();
        }

        public JobHandle UpdateView(NativeArray<BulletData> bulletDataArray, JobHandle dependenceJobHandle)
        {
            var emit = bulletDataArray.Length - _particleSystem.particleCount;
            if (emit > 0)
                _particleSystem.Emit(new ParticleSystem.EmitParams()
                {
                    startSize = 0.1f
                }, emit);

            return new ParticleJob()
            {
                BulletDataArray = bulletDataArray
            }.Schedule(_particleSystem, 16, dependenceJobHandle);
        }


        public void Dispose()
        {
            
        }

        [BurstCompile]
        private struct ParticleJob : IJobParticleSystemParallelFor
        {
            [ReadOnly] public NativeArray<BulletData> BulletDataArray;

            public void Execute(ParticleSystemJobData jobData, int index)
            {
                if (index >= BulletDataArray.Length)
                {
                    var lifetime = jobData.aliveTimePercent;
                    lifetime[index] = 100f;
                    return;
                }

                var data = BulletDataArray[index];

                if (!data.IsActive)
                {
                    var lifetime = jobData.aliveTimePercent;
                    lifetime[index] = 100f;
                    return;
                }

                var positions = jobData.positions;
                positions[index] = data.Position;

                var colors = jobData.startColors;

                colors[index] = !data.IsEnemy ? data.Color : Color.red;
            }
        }
    }
}