using System.Linq;
using DG.Tweening;
using Moon.Bullet;
using Moon.Item;
using Moon.Shooter;
using TMPro;
using UnityEngine;
using VContainer;

namespace Moon.Result
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private TMP_Text itemText;

        [SerializeField] private CanvasGroup mainUI;


        [Inject] private ItemDatabase _itemDatabase;


        private CanvasGroup _canvasGroup;


        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            gameObject.SetActive(false);
        }

        public void Show(int stage, ShooterHolder shooterHolder)
        {
            mainUI.DOFade(0, 1f);
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1f, 1f);

            stageText.text = "第" + stage + "層";

            var text = string.Empty;

            foreach (var (key, i) in shooterHolder.ShooterLevels)
            {
                text += _itemDatabase.ItemDataArray.First(value => value.ShooterType == key)
                    .Name;

                text += " ";

                if (key != ShooterType.Moon)
                {
                    text += "Lv." + i;
                }
                else
                {
                    text += i + "/5";
                }

                text += "\n";
            }

            itemText.text = text;
        }
    }
}