using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace Moon
{
    public class Bullet : MonoBehaviour
    {
        private JobHandle _jobHandle;

        private ParticleSystem _particleSystem;


        // Start is called before the first frame update
        void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            _jobHandle.Complete();

            _jobHandle = new ParticleJob().Schedule(_particleSystem, 1, _jobHandle);
            JobHandle.ScheduleBatchedJobs();
        }

        private struct BulletParameter
        {
            public Vector2 Position;
            public Vector2 Direction;
        }

        [BurstCompile]
        private struct ParticleJob : IJobParticleSystemParallelFor
        {
            public float DeltaTime;
            public NativeArray<BulletParameter> BulletParameters;

            public void Execute(ParticleSystemJobData jobData, int index)
            {
                
            }
        }
    }
}