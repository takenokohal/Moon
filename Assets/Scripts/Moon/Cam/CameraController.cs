using UnityEngine;

namespace Moon.Cam
{
    public class CameraController : MonoBehaviour
    {
        private Transform _player;

        // Start is called before the first frame update
        void Start()
        {
            _player = GameObject.Find("Player").transform;
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