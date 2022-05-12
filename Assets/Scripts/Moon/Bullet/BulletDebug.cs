using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;

namespace Moon.Bullet
{
    public class BulletDebug : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;

        [Inject] private IBulletManager _bulletManager;

        private void Start()
        {
            Loop().Forget();
        }

        private async UniTaskVoid Loop()
        {
            var token = this.GetCancellationTokenOnDestroy();
            while (!token.IsCancellationRequested)
            {
                tmpText.text = _bulletManager.BulletDataArray.Length.ToString();
                await UniTask.Delay(100, cancellationToken: token);
            }
        }
    }
}