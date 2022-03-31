using DG.Tweening;
using Moon.Bullet;
using UnityEngine;
using VContainer;

namespace Moon.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private BulletManager _bulletManager;

        private int _life = 20;

        private ParticleSystem _particleSystem;

        public void Construct(BulletManager bulletManager)
        {
            _bulletManager = bulletManager;
            _particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            var list = _bulletManager.OriginBulletList;

            var v = list.RemoveAll(value =>
            {
                if (value.IsEnemy)
                    return false;
                return Vector2.Distance(transform.position, value.Position) < 0.1f;
            });

            if (v <= 0)
                return;

            _life -= v;
            //  var f = 1f - _life / 10f;
            //   var main = _particleSystem.main;
            //   main.startColor = new ParticleSystem.MinMaxGradient(new Color(f, f, f));
            transform.DOShakePosition(0.1f, 0.5f);

            if (_life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}