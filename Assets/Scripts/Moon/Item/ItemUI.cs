using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Moon.Bullet;
using Moon.Player;
using Moon.Shooter;
using UniRx;
using UnityEngine;
using VContainer;

namespace Moon.Item
{
    public class ItemUI : MonoBehaviour
    {
        [Inject] private PlayerCore _playerCore;
        [SerializeField] private ItemIcon prefab;

        private readonly Dictionary<ShooterType, ItemIcon> _itemIcons = new();

        [Inject] private ItemDatabase _itemDatabase;

        private void Start()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            await UniTask.WaitWhile(() => _playerCore.ShooterHolder == null);

            foreach (var (key, value) in _playerCore.ShooterHolder.ShooterLevels)
            {
                AddIcon(key);
                SetLevel(key, value);
            }

            _playerCore.ShooterHolder.ShooterLevels.ObserveAdd().Subscribe(value =>
            {
                AddIcon(value.Key);
                SetLevel(value.Key, value.Value);
            }).AddTo(gameObject);

            _playerCore.ShooterHolder.ShooterLevels.ObserveReplace().Subscribe(value =>
            {
                SetLevel(value.Key, value.NewValue);
            }).AddTo(gameObject);
        }

        private void AddIcon(ShooterType shooterType)
        {
            var inst = Instantiate(prefab);
            inst.SetSprite(_itemDatabase.ItemDataArray.First(value => value.ShooterType == shooterType).Sprite);
            _itemIcons.Add(shooterType, inst);


            var t = inst.transform;
            t.SetParent(transform);
            t.localPosition = new Vector3((_itemIcons.Count - 1) * -25, 0);
            t.localScale = Vector3.one * 0.2f;
        }

        private void SetLevel(ShooterType shooterType, int level)
        {
            _itemIcons[shooterType].SetLevel(level);
        }
    }
}