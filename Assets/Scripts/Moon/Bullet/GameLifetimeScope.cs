using Moon.Audio;
using Moon.Enemy;
using Moon.Item;
using Moon.Player;
using Moon.Result;
using Moon.Stage;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Moon.Bullet
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyController enemyControllerPrefab;

        [SerializeField] private ItemObject itemObject;


        [SerializeField] private ItemDatabase itemDatabase;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameLoop>();

            builder.RegisterComponentInHierarchy<BulletManager>().As<IBulletManager>();

            builder.RegisterComponentInHierarchy<PlayerCore>();

            builder.RegisterComponent(enemyControllerPrefab);

            builder.Register<EnemyController.Factory>(Lifetime.Singleton);

            builder.Register<EnemySpawner>(Lifetime.Singleton);

            builder.RegisterInstance(itemDatabase);

            builder.Register<ItemManager>(Lifetime.Singleton);

            builder.Register<ItemObject.Factory>(Lifetime.Singleton);

            builder.RegisterInstance(itemObject);

            builder.RegisterComponentInHierarchy<ResultUI>();

            builder.RegisterComponentInHierarchy<StageUI>();

            builder.RegisterComponentInHierarchy<ClearUI>();
        }
    }
}