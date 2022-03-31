using Moon.Bullet;
using UnityEngine;

namespace Moon.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyController enemy;

        private void Start()
        {
            var bulletManager = FindObjectOfType<BulletManager>();
            for (int i = 0; i < 100; i++)
            {
                var e = Instantiate(enemy);
                e.Construct(bulletManager);
                var t = e.transform;
                t.position = Random.insideUnitCircle * 20;
            }
        }
    }
}