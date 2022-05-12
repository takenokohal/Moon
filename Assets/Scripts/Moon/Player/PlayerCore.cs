using System;
using DG.Tweening;
using Moon.Audio;
using Moon.Bullet;
using Moon.Common;
using Moon.Item;
using Moon.Shooter;
using UniRx;
using UnityEngine;
using VContainer;

namespace Moon.Player
{
    public class PlayerCore : MonoBehaviour, IDamageable
    {
        public const int MaxLifePoint = 5;
        private readonly ReactiveProperty<int> _lifePoint = new(MaxLifePoint);

        public int LifePoint
        {
            get => _lifePoint.Value;
            private set => _lifePoint.Value = value;
        }

        public IObservable<int> LifePointObservable => _lifePoint;

        private readonly ReactiveProperty<bool> _isDead = new();
        public bool IsDead => _isDead.Value;
        public IObservable<Unit> OnDeadObservable => _isDead.Where(value => value).AsUnitObservable();

        public ShooterHolder ShooterHolder { get; private set; }

        [Inject] private IBulletManager _bulletManager;
        [Inject] private ItemManager _itemManager;

        [SerializeField] private Renderer myRenderer;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private Color _defaultColor;


        private void Start()
        {
            ShooterHolder = new ShooterHolder(() => transform.position, false);

            ShooterHolder.AddShooter(ShooterType.Moon);

            for (int i = 0; i < 3; i++)
            {
                ShooterHolder.AddShooter(EnumExtension.GetRandomValue<ShooterType>(1, 1));
            }

            _bulletManager.RegisterShooterHolder(ShooterHolder);

            _bulletManager.RegisterDamageable(this);

            _defaultColor = myRenderer.material.GetColor(EmissionColor);


            LifePointObservable.Subscribe(value =>
            {
                if (value != 0)
                    return;

                _isDead.Value = true;
                _bulletManager.UnRegisterDamageable(this);
                _bulletManager.UnRegisterShooterHolder(ShooterHolder);
            });
        }

        public void AddShooter(ShooterType shooterType)
        {
            ShooterHolder.AddShooter(shooterType);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public void OnDamaged(int damage)
        {
            myRenderer.material.SetColor(EmissionColor, Color.red * 5);
            LifePoint--;
            Camera.main.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => myRenderer.material.SetColor(EmissionColor, _defaultColor));

            SoundEffectManager.Play(SoundEffectManager.SoundEffectType.Attacked);
        }

        public bool IsEnemy => false;
    }
}