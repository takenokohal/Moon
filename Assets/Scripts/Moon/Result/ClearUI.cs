using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Moon.Bullet;
using Moon.Item;
using Moon.Shooter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Moon.Result
{
    public class ClearUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private TMP_Text itemText;

        [Inject] private ItemDatabase _itemDatabase;

        [SerializeField] private Image panel;
        [SerializeField] private TMP_Text clearText;
        [SerializeField] private CanvasGroup result;


        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async UniTask Show(int stage, ShooterHolder shooterHolder)
        {
            gameObject.SetActive(true);
            await panel.DOFade(1f, 1f);
            await UniTask.Delay(500);

            await clearText.DOFade(1, 1);
            await UniTask.Delay(2000);
            await clearText.DOFade(0, 1);

            stageText.text = "第" + stage + "層";

            var text = string.Empty;
            foreach (var (key, i) in shooterHolder.ShooterLevels)
            {
                if (key == ShooterType.Moon)
                    continue;

                text += _itemDatabase.ItemDataArray.First(value => value.ShooterType == key)
                    .Name;

                text += " ";

                text += "Lv." + i;
                text += "\n";
            }

            itemText.text = text;

            await result.DOFade(1, 1);
        }
    }
}