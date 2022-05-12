using System;
using System.Linq;
using Moon.Bullet;
using Moon.Player;
using Moon.Shooter;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Moon.Item
{
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem particle;


        private readonly ReactiveProperty<bool> _isHitting = new();
        public IObservable<bool> IsHittingObservable => _isHitting;

        public class Factory
        {
            private readonly ItemObject _prefab;
            private readonly ItemDatabase _itemDatabase;

            public Factory(ItemObject itemObject, ItemDatabase itemDatabase)
            {
                _prefab = itemObject;
                _itemDatabase = itemDatabase;
            }

            public ItemObject Create(ShooterType shooterType, PlayerCore playerCore)
            {
                var inst = Instantiate(_prefab);

                var data = _itemDatabase.ItemDataArray.First(value => value.ShooterType == shooterType);
                inst.Construct(data, playerCore);
                return inst;
            }
        }

        private void Construct(ItemDatabase.ItemData data, PlayerCore playerCore)
        {
            var main = particle.main;
            main.startColor = data.Color;

            spriteRenderer.color = data.Color;
            spriteRenderer.sprite = data.Sprite;

            this.FixedUpdateAsObservable()
                .Select(_ => (playerCore.GetPosition() - (Vector2)transform.position).sqrMagnitude < 0.5f)
                .Subscribe(value => _isHitting.Value = value)
                .AddTo(this);
        }
    }
}