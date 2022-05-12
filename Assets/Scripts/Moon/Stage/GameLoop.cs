using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Moon.Bullet;
using Moon.Common;
using Moon.Enemy;
using Moon.Item;
using Moon.Player;
using Moon.Result;
using Moon.Shooter;
using UniRx;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Moon.Stage
{
    public class GameLoop : IDisposable, IInitializable
    {
        [Inject] private PlayerCore _playerCore;

        private readonly List<EnemyController> _enemies = new();
        [Inject] private readonly EnemySpawner _enemySpawner;

        [Inject] private readonly ItemManager _itemManager;

        [Inject] private readonly IBulletManager _bulletManager;

        [Inject] private ResultUI _resultUI;
        [Inject] private ClearUI _clearUI;

        [Inject] private StageUI _stageUI;

        private int _stageLevel = 1;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private async UniTaskVoid Loop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await _stageUI.ChangeStage(_stageLevel);

                _enemies.Clear();
                //敵の生成

                var list = await _enemySpawner.Spawn(_stageLevel);
                _enemies.AddRange(list);

                _bulletManager.StartShooting();

                await UniTask.WaitUntil(() => _enemies.All(value => value == null),
                    cancellationToken: _cancellationTokenSource.Token);

                _bulletManager.StopShooting();

                //報酬
                _itemManager.CreateItems(_playerCore);
                var item = await _itemManager.WaitGetItem();
                _playerCore.AddShooter(item);

                if (_playerCore.ShooterHolder.TryGet(ShooterType.Moon, out var value))
                {
                    if (value >= 5)
                    {
                        Clear().Forget();
                        return;
                    }
                }

                _stageLevel++;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Initialize()
        {
            Loop().Forget();

            _playerCore.OnDeadObservable.Subscribe(_ => GameOver().Forget());
        }

        private async UniTask GameOver()
        {
            _cancellationTokenSource.Cancel();

            _resultUI.Show(_stageLevel, _playerCore.ShooterHolder);

            await UniTask.WaitUntil(() => Keyboard.current.enterKey.wasPressedThisFrame);

            SceneFader.FadeScene("Title").Forget();
        }

        private async UniTask Clear()
        {
            _cancellationTokenSource.Cancel();

            await _clearUI.Show(_stageLevel, _playerCore.ShooterHolder);

            await UniTask.WaitUntil(() => Keyboard.current.enterKey.wasPressedThisFrame);

            SceneFader.FadeScene("Title").Forget();
        }
    }
}