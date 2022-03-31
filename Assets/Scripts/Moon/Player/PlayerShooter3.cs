using Cysharp.Threading.Tasks;
using Moon.Bullet;
using UnityEngine;

namespace Moon.Player
{
    public class PlayerShooter3 : MonoBehaviour
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
                var rot = _currentRotation;
                _bulletManager.AddBullet(new BulletManager.BulletData()
                {
                    Position = transform.position,
                    Velocity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * 3,
                    ColorHue = 0.3f,
                    Lifetime = 1
                });


                _currentRotation += 0.1f;
                if (_currentRotation >= Mathf.PI * 2)
                    _currentRotation = 0f;

                await UniTask.Delay(10);
            }
        }
    }
}