using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Moon.Bullet;
using Moon.Player;
using Moon.Shooter;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Moon.Item
{
    public class ItemGetUI : MonoBehaviour
    {
        [Inject] private ItemManager _itemManager;
        [Inject] private PlayerCore _playerCore;
        [Inject] private ItemDatabase _itemDatabase;

        [SerializeField] private TMP_Text shooterNameText;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text levelText;


        private CanvasGroup _canvasGroup;

        private void Start()
        {
            gameObject.SetActive(false);

            _itemManager.CurrentItem.Subscribe(value => Show(value).Forget());
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private async UniTaskVoid Show(ShooterType shooterType)
        {
            if (shooterType == ShooterType.None)
            {
                await _canvasGroup.DOFade(0, 0.2f);
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            var holder = _playerCore.ShooterHolder;

            var data = _itemDatabase.ItemDataArray.First(itemData => itemData.ShooterType == shooterType);
            shooterNameText.text = data.Name;
            icon.sprite = data.Sprite;
            if (data.ShooterType == ShooterType.Moon)
            {
                levelText.text = holder.TryGet(shooterType, out var value)
                    ? value+ "/5"
                    : "NEW";
            }
            else
            {
                levelText.text = holder.TryGet(shooterType, out var value)
                    ? "Lv." + value + " -> " + "Lv." + (value + 1)
                    : "NEW";
            }

            await _canvasGroup.DOFade(0, 0);
            _canvasGroup.DOFade(1, 0.2f);
        }
    }
}