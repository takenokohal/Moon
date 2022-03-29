using Cysharp.Threading.Tasks;
using Moon.Bullet;
using UnityEngine;

namespace Moon.Player
{
    public class PlayerShooter : MonoBehaviour
    {
        private BulletManager _bulletManager;

        private float _currentRotation;

        private void Start()
        {
            _bulletManager = FindObjectOfType<BulletManager>();
            Loop().Forget();
        }


        private async UniTaskVoid Loop()
        {
            while (gameObject.activeInHierarchy)
            {
                const int length = 3;
                for (int i = 0; i < length; i++)
                {
                    var rot = _currentRotation + 2 * Mathf.PI / length * i;
                    _bulletManager.AddBullet(new BulletManager.BulletData()
                    {
                        Position = transform.position,
                        Velocity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * 2,
                        ColorHue = 0.1f
                    });
                }

                for (int i = 0; i < length; i++)
                {
                    var rot = -(_currentRotation + 2 * Mathf.PI / length * i);
                    _bulletManager.AddBullet(new BulletManager.BulletData()
                    {
                        Position = transform.position,
                        Velocity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * 2,
                        ColorHue = 0.1f
                    });
                }


                _currentRotation += 0.1f;
                if (_currentRotation >= Mathf.PI * 2)
                    _currentRotation = 0f;

                await UniTask.Delay(100);
            }
        }
    }
}