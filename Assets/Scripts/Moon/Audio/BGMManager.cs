using UnityEngine;

namespace Moon.Audio
{
    public class BGMManager : MonoBehaviour
    {
        private static bool _instanced;

        // Start is called before the first frame update
        private void Start()
        {
            if (!_instanced)
            {
                _instanced = true;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}