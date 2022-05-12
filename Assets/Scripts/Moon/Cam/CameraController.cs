using Moon.Player;
using UniRx;
using UnityEngine;

namespace Moon.Cam
{
    public class CameraController : MonoBehaviour
    {
        private PlayerCore _player;

        // Start is called before the first frame update
        void Start()
        {
            _player = FindObjectOfType<PlayerCore>();

            _player.OnDeadObservable.Subscribe(_ => Destroy(this));
        }

        // Update is called once per frame
        void Update()
        {
            var pos = transform.position;
            var next = Vector3.Lerp(pos, _player.transform.position, 0.01f);
            next.z = pos.z;
            transform.position = next;
        }
    }
}