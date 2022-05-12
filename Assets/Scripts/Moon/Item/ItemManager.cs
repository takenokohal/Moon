using System;
using Cysharp.Threading.Tasks;
using Moon.Common;
using Moon.Player;
using Moon.Shooter;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Object = UnityEngine.Object;

namespace Moon.Item
{
    public class ItemManager
    {
        [Inject] private readonly ItemObject.Factory _factory;

        private readonly ReactiveProperty<ShooterType> _currentItem = new();
        public IObservable<ShooterType> CurrentItem => _currentItem;


        private readonly ItemObject[] _itemObjects = new ItemObject[5];

        public async UniTask<ShooterType> WaitGetItem()
        {
            await UniTask.WaitUntil(() =>
                _currentItem.Value != ShooterType.None && Keyboard.current.enterKey.wasPressedThisFrame);

            foreach (var itemObject in _itemObjects)
            {
                Object.Destroy(itemObject.gameObject);
            }

            var type = _currentItem.Value;
            _currentItem.Value = ShooterType.None;
            return type;
        }

        public void CreateItems(PlayerCore playerCore)
        {
            const int length = 5;
            var array = new IObservable<ShooterType>[length];
            for (int i = 0; i < length; i++)
            {
                var itemType = i != 0 ? EnumExtension.GetRandomValue<ShooterType>(1, 1) : ShooterType.Moon;
                var v = _factory.Create(itemType, playerCore);
                _itemObjects[i] = v;
                v.transform.position =
                    (Vector3)new Circular(2f, i * Mathf.PI * 2f / length + 2 * Mathf.PI / 4f).ToVector() +
                    playerCore.transform.position;

                array[i] = v.IsHittingObservable.Select(_ => itemType).TakeUntilDestroy(v);

                v.IsHittingObservable.Subscribe(value => _currentItem.Value = value ? itemType : ShooterType.None)
                    .AddTo(v);
            }
        }
    }
}