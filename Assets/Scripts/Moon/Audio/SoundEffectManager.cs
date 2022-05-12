using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moon.Audio
{
    public class SoundEffectManager : SerializedMonoBehaviour
    {
        private static SoundEffectManager _instance;

        [SerializeField] private Dictionary<SoundEffectType, AudioClip> _clips;

        private AudioSource _audioSource;

        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _audioSource = GetComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void Play(SoundEffectType soundEffectType)
        {
            _instance._audioSource.PlayOneShot(_instance._clips[soundEffectType]);
        }

        public enum SoundEffectType
        {
            Start,
            Attack,
            Attacked,
            Kill,
            GetItem,
        }
    }
}