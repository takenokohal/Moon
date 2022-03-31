using Cysharp.Threading.Tasks;
using Moon.Bullet;
using UnityEngine;

namespace Moon.Enemy
{
    public class EnemyShooter : MonoBehaviour
    {
        private BulletManager _bulletManager;

        private float _currentRotation;

        private void Start()
        {
            _bulletManager = FindObjectOfType<BulletManager>();
            Loop().Forget();

            _currentRotation = Random.Range(0, 2 * Mathf.PI);
        }


        private async UniTaskVoid Loop()
        {
            while (gameObject.activeInHierarchy)
            {
                var rot = _currentRotation;
                _bulletManager.AddBullet(new BulletManager.BulletData()
                {
                    Position = transform.position,
                    Velocity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)),
                    ColorHue = 0f,
                    Lifetime = 1,
                    IsEnemy = true
                });


                _currentRotation += 0.2f;
                if (_currentRotation >= Mathf.PI * 2)
                    _currentRotation = 0f;

                await UniTask.Delay(100, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            }
        }
    }
}