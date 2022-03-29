using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Moon.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float speed;

        private PlayerInput _playerInput;


        private void Start()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
          //  Application.targetFrameRate = 30;
        }

        private void Update()
        {
            var v = _playerInput.currentActionMap["Move"].ReadValue<Vector2>();
            transform.position += new Vector3(v.x, v.y) * speed * Time.deltaTime;
        }
    }
}