using Cysharp.Threading.Tasks;
using Moon.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moon.Title
{
    public class TitleController : MonoBehaviour
    {
        private void Start()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            await UniTask.WaitUntil(() => Keyboard.current.enterKey.wasPressedThisFrame);
            SceneFader.FadeScene("GameScene").Forget();
        }
    }
}