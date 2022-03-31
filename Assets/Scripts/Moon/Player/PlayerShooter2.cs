using Cysharp.Threading.Tasks;
using Moon.Bullet;
using UnityEngine;

namespace Moon.Player
{
    public class PlayerShooter2 : MonoBehaviour
    {
        private BulletManager _bulletManager;


        private void Start()
        {
            _bulletManager = FindObjectOfType<BulletManager>();
            Loop().Forget();
        }

        private async UniTaskVoid Loop()
        {
            while (gameObject.activeInHierarchy)
            {
                const int length = 36;
                for (int i = 0; i < length; i++)
                {
                    var rot = 2 * Mathf.PI / length * i;
                    _bulletManager.AddBullet(new BulletManager.BulletData()
                    {
                        Position = transform.position,
                        Velocity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * 2,
                        ColorHue = 0.5f,
                        Lifetime = 1
                    });
                }

                await UniTask.Delay(1000);
            }
        }
    }
}