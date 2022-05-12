using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Moon.Common
{
    public class SceneFader : MonoBehaviour
    {
        private Image _panel;

        private static SceneFader _instance;

        private void Start()
        {
            if (_instance == null)
            {
                _panel = GetComponentInChildren<Image>();
                GameObject o;
                (o = gameObject).SetActive(false);

                _instance = this;
                DontDestroyOnLoad(o);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static async UniTask FadeScene(string sceneName)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            loadSceneAsync.allowSceneActivation = false;

            _instance.gameObject.SetActive(true);

            await _instance._panel.DOFade(0, 0);
            await _instance._panel.DOFade(1f, 1f);
            await UniTask.Delay(1000);

            loadSceneAsync.allowSceneActivation = true;
            await loadSceneAsync;

            await _instance._panel.DOFade(0f, 1f);

            _instance.gameObject.SetActive(false);
        }
    }
}