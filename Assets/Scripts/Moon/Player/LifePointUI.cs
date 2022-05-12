using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Moon.Player
{
    public class LifePointUI : MonoBehaviour
    {
        [Inject] private PlayerCore _playerCore;

        [SerializeField] private Slider slider;


        private void Start()
        {
            _playerCore.LifePointObservable.Subscribe(
                value => { slider.value = (float)value / PlayerCore.MaxLifePoint; });
        }
    }
}