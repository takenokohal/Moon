using Moon.Bullet;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VRM;

namespace Moon.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float speed;

        private PlayerInput _playerInput;

        private PlayerCore _playerCore;


        private void Start()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
            _playerCore = GetComponent<PlayerCore>();

            FastSpringBoneReplacer.ReplaceAsync(gameObject);
        }

        private void Update()
        {
            if (_playerCore != null)
                if (_playerCore.IsDead)
                {
                    transform.position += new Vector3(0, -1) * Time.deltaTime * speed;
                    return;
                }

            var v = _playerInput.currentActionMap["Move"].ReadValue<Vector2>();
            transform.position += new Vector3(v.x, v.y) * speed * Time.deltaTime;
        }
    }
}