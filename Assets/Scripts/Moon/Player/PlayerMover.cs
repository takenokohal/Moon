using UnityEngine;
using UnityEngine.InputSystem;
using VRM;

namespace Moon.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float speed;

        private PlayerInput _playerInput;


        private void Start()
        {
            _playerInput = FindObjectOfType<PlayerInput>();

            FastSpringBoneReplacer.ReplaceAsync(gameObject);
        }

        private void Update()
        {
            var v = _playerInput.currentActionMap["Move"].ReadValue<Vector2>();
            transform.position += new Vector3(v.x, v.y) * speed * Time.deltaTime;
        }
    }
}