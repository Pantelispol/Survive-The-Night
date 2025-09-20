using UnityEngine;


namespace Script.Controller
{
    [DefaultExecutionOrder(-3)]
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance { get; private set; }
        public PlayerControls PlayerControls { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerControls = new PlayerControls();
        }
        private void OnEnable()
        {
            if (PlayerControls != null)
                PlayerControls.Enable();
        }

        private void OnDisable()
        {
            if (PlayerControls != null)
                PlayerControls.Disable();
        }
    }
}
