using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Moon.Enemy
{
    public class EnemyExplosionViewer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explosionParticle;

        public async UniTaskVoid PlayExplosion(Vector2 position)
        {
            var v = Instantiate(explosionParticle, position, Quaternion.identity);
            await UniTask.WaitWhile(() => v.IsAlive());
            Destroy(v);
        }
    }
}