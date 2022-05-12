using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moon.Item;
using Moon.Shooter;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VContainer;

namespace Moon.Bullet
{
    public class BulletManager : MonoBehaviour, IBulletManager
    {
        public const int BulletPerSecond = 100;

        //  private BulletView _bulletView;
        private BulletParticleView _bulletParticleView;

        public NativeArray<BulletData> BulletDataArray { get; private set; }
        private readonly List<BulletData> _tmpBulletDataList = new(1000000);

        private NativeArray<DamagedData> _damagedDataArray;

        private JobHandle _bulletJobHandle;
        private JobHandle _viewJobHandle;

        [SerializeField] private Material material;

        private readonly List<ShooterHolder> _shooters = new();
        private readonly List<IDamageable> _damageableList = new();

        private bool _isStop = true;

        [Inject] private ItemDatabase _itemDatabase;

        public void RegisterShooterHolder(ShooterHolder shooter) => _shooters.Add(shooter);

        public void RegisterDamageable(IDamageable damageable) => _damageableList.Add(damageable);

        public void UnRegisterShooterHolder(ShooterHolder shooter)
        {
            _shooters.Remove(shooter);
        }

        public void UnRegisterDamageable(IDamageable damageable)
        {
            _damageableList.Remove(damageable);
        }

        private Transform _cameraPosition;

        private void Start()
        {
            //   _bulletView = new BulletView(material);
            _bulletParticleView = new BulletParticleView(GetComponent<ParticleSystem>());

            _cameraPosition = Camera.main.transform;
        }

        private void OnDisable()
        {
            _viewJobHandle.Complete();
            _bulletParticleView.Dispose();
            BulletDataArray.Dispose(_bulletJobHandle);
            _damagedDataArray.Dispose(_bulletJobHandle);
        }

        public void StopShooting()
        {
            _isStop = true;
            _tmpBulletDataList.Clear();
            CreateTmpArray();
        }

        public void StartShooting()
        {
            _isStop = false;
        }

        private void Update()
        {
            JobHandle.CompleteAll(ref _bulletJobHandle, ref _viewJobHandle);
            if (BulletDataArray.IsCreated)
            {
                CreateList();
            }

            if (_damagedDataArray.IsCreated)
                for (int i = 0; i < _damagedDataArray.Length; i++)
                {
                    var damage = _damagedDataArray[i].Damage;
                    if (damage > 0)
                    {
                        _damageableList[i].OnDamaged(damage);
                    }
                }

            //////////


            if (!_isStop)
                foreach (var shooter in _shooters)
                {
                    shooter.ShooterUpdate(this);
                }

            CreateTmpArray();

            _bulletJobHandle = new BulletJob
            {
                BulletDataArray = BulletDataArray,
                DeltaTime = Time.deltaTime,
                DamagedDataArray = _damagedDataArray,
                CameraPosition = _cameraPosition.position
            }.Schedule(BulletDataArray.Length, 16);

            _viewJobHandle = _bulletParticleView.UpdateView(BulletDataArray, _bulletJobHandle);

            JobHandle.ScheduleBatchedJobs();
        }


        private void CreateTmpArray()
        {
            if (BulletDataArray.IsCreated)
                BulletDataArray.Dispose();


            BulletDataArray = new NativeArray<BulletData>(_tmpBulletDataList.Count, Allocator.TempJob);
            for (var i = 0; i < _tmpBulletDataList.Count; i++)
            {
                var bulletDataArray = BulletDataArray;
                bulletDataArray[i] = _tmpBulletDataList[i];
            }
            //////

            if (_damagedDataArray.IsCreated)
                _damagedDataArray.Dispose();

            _damagedDataArray = new NativeArray<DamagedData>(_damageableList.Count, Allocator.TempJob);

            for (int i = 0; i < _damagedDataArray.Length; i++)
            {
                _damagedDataArray[i] = new DamagedData()
                {
                    Position = _damageableList[i].GetPosition(),
                    IsEnemy = _damageableList[i].IsEnemy
                };
            }
        }

        private void CreateList()
        {
            _tmpBulletDataList.Clear();

            if (BulletDataArray.Any())
                _tmpBulletDataList.AddRange(BulletDataArray.Where(value => value.IsActive));
        }

        public void AddBullet(Vector2 position, Vector2 velocity, ShooterType shooterType, bool isEnemy)
        {
            var v = new BulletData(position, velocity,
                _itemDatabase.ItemDataArray.First(value => value.ShooterType == shooterType).Color, isEnemy);
            _tmpBulletDataList.Add(v);
        }


        [BurstCompile]
        private struct BulletJob : IJobParallelFor
        {
            [NativeDisableParallelForRestriction] public NativeArray<BulletData> BulletDataArray;
            [NativeDisableParallelForRestriction] public NativeArray<DamagedData> DamagedDataArray;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public Vector2 CameraPosition;

            private const float MaxTime = 30;
            private const float MaxRange = 256;

            public void Execute(int index)
            {
                var data = BulletDataArray[index];
                if (!data.IsActive)
                    return;

                data.Position += data.Velocity * DeltaTime;
                data.ElapsedTime += DeltaTime;

                if (data.ElapsedTime > MaxTime)
                {
                    Destroy(index);
                    return;
                }


                var dp = CameraPosition - data.Position;
                if (dp.sqrMagnitude > MaxRange)
                {
                    if (Vector2.Dot(dp, data.Velocity) < 0)
                    {
                        Destroy(index);
                        return;
                    }
                }


                for (int i = 0; i < DamagedDataArray.Length; i++)
                {
                    var damageData = DamagedDataArray[i];
                    if (data.IsEnemy == damageData.IsEnemy)
                        continue;
                    if ((data.Position - damageData.Position).sqrMagnitude > 0.25f)
                        continue;

                    damageData.Damage++;
                    DamagedDataArray[i] = damageData;
                    Destroy(index);
                    return;
                }


                for (int i = 0; i < BulletDataArray.Length; i++)
                {
                    if (i == index)
                        continue;
                    var v = BulletDataArray[i];
                    if (data.IsEnemy == v.IsEnemy)
                        continue;

                    if (!v.IsActive)
                        continue;

                    if ((v.Position - data.Position).sqrMagnitude > 0.01f)
                        continue;

                    v.IsActive = false;
                    Destroy(index);
                    return;
                }


                BulletDataArray[index] = data;
            }

            private void Destroy(int index)
            {
                var v = BulletDataArray[index];
                v.IsActive = false;
                BulletDataArray[index] = v;
            }
        }

        private struct DamagedData
        {
            public Vector2 Position;
            public int Damage;
            public bool IsEnemy;
        }
    }
}