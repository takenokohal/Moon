using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Moon.Stage
{
    public class StageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;

        [SerializeField] private TMP_Text changeText;

        public async UniTask ChangeStage(int i)
        {
            var str = "第" + i + "層";
            tmpText.text = str;

            changeText.text = str;
            changeText.gameObject.SetActive(true);
            await changeText.DOFade(0, 0);
            await changeText.DOFade(1, 0.5f);
            await UniTask.Delay(2000);
            await changeText.DOFade(0, 0.5f);
            changeText.gameObject.SetActive(true);
        }
    }
}