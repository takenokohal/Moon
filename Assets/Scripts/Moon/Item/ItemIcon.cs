using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moon.Item
{
    public class ItemIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;

        public void SetSprite(Sprite sprite) => image.sprite = sprite;

        public void SetLevel(int level) => text.text = level.ToString();
    }
}