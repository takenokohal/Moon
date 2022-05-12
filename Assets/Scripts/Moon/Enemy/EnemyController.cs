using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Moon.Audio;
using Moon.Bullet;
using Moon.Shooter;
using UniRx.Triggers;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using VContainer;

namespace Moon.Enemy
{
    public class EnemyController : MonoBehaviour, IDamageable
    {
        private int _life;

        private ParticleSystem _particleSystem;

        private IBulletManager _bulletManager;

        private ShooterHolder _shooterHolder;

        [SerializeField] private ParticleSystem exp;

        public void AddShooter(ShooterType shooterType) => _shooterHolder.AddShooter(shooterType);

        private NativeArray<int> _damageResult;
        private JobHandle _damageJobHandle;
        public bool IsEnemy => true;

        public class Factory
        {
            [Inject] private readonly IBulletManager _bulletManager;
            [Inject] private EnemyController _enemyControllerPrefab;

            public EnemyController Create(int life)
            {
                var v = Instantiate(_enemyControllerPrefab);
                v.Construct(_bulletManager, life);

                return v;
            }
        }

        private void Construct(IBulletManager bulletManager, int life)
        {
            _life = life;
            _bulletManager = bulletManager;
            _shooterHolder = new ShooterHolder(() => transform.position, true);

            _bulletManager.RegisterDamageable(this);
            _bulletManager.RegisterShooterHolder(_shooterHolder);

            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Destroy()
        {
            _bulletManager.UnRegisterShooterHolder(_shooterHolder);
            _bulletManager.UnRegisterDamageable(this);

            Destroy(gameObject);
        }


        private struct Job : IJobParticleSystemParallelFor
        {
            public Color32 Color;

            public void Execute(ParticleSystemJobData jobData, int index)
            {
                var v = jobData.startColors;
                v[index] = Color;
            }
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public void OnDamaged(int damage)
        {
            _life -= damage;
            if (_life <= 0)
            {
                Kill().Forget();
                return;
            }

            OnDamagedAsync().Forget();
        }


        private async UniTaskVoid OnDamagedAsync()
        {
            SoundEffectManager.Play(SoundEffectManager.SoundEffectType.Attack);
            var token = this.GetCancellationTokenOnDestroy();
            var handle = new Job { Color = Color.white }.Schedule(_particleSystem, 32);
            JobHandle.ScheduleBatchedJobs();
            await UniTask.Yield(cancellationToken: token);
            handle.Complete();
            await UniTask.Delay(100, cancellationToken: token);


            var handle2 = new Job { Color = Color.black }.Schedule(_particleSystem, 32);
            JobHandle.ScheduleBatchedJobs();
            await UniTask.Yield(token);
            handle2.Complete();
        }

        private async UniTaskVoid Kill()
        {
            SoundEffectManager.Play(SoundEffectManager.SoundEffectType.Kill);
            Camera.main.DOShakePosition(0.5f, 0.5f);
            var v = Instantiate(exp, transform.position, Quaternion.identity);
            Destroy();
            await UniTask.WaitWhile(() => v.IsAlive(true), cancellationToken: v.GetCancellationTokenOnDestroy());
            Destroy(v.gameObject);
        }
    }
}