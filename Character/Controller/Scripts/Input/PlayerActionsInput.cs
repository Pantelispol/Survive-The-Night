using Script.Controller;
using SmallHedge.SoundManager;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Controller
{
    [DefaultExecutionOrder(-2)]
    public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionMapActions
    {
        #region Class Variables 
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private CombatState _combatState;
        private PlayerInventory _playerInventory;
        [SerializeField] private Transform Camera;
        [SerializeField] private LayerMask UseLayers;
        [SerializeField] private float MaxUseDistance = 5f;
        [SerializeField] private TextMeshPro UseText;
        public bool AttackPressed { get; private set; }
        public bool GatherPressed { get; private set; }

        #endregion

        #region StartUp 

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _combatState = GetComponent<CombatState>();
            _playerInventory = GetComponent<PlayerInventory>();
        }
        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.RemoveCallbacks(this);
        }
        #endregion

        #region Update
        private void Update()
        {
            if (_playerLocomotionInput.MovementInput != Vector2.zero ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling)
            {
                GatherPressed = false;
            }
            if (UseText == null) return;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, out RaycastHit hit, MaxUseDistance, UseLayers)
            && hit.collider.TryGetComponent<Door>(out Door door))
            {
                if (door.IsOpen)
                {
                    UseText.SetText("Close \"I\"");
                }
                else
                {
                    UseText.SetText("Open \"I\"");
                }
                UseText.gameObject.SetActive(true);
                UseText.transform.position = hit.point - (hit.point - Camera.position).normalized * 0.01f;
                UseText.transform.rotation = Quaternion.LookRotation((hit.point - Camera.position).normalized);
            }
            else
            {
                UseText.gameObject.SetActive(false);
            }
        }
        public void SetGatherPressedFalse()
        {
            GatherPressed = false;
        }

        public void SetAttackPressedFalse()
        {
            AttackPressed = false;
        }

        #endregion

        #region Input Callbacks
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            AttackPressed = true;
            if (_playerInventory.IsHoldingSword())
            {
                _combatState.GetComponent<Animator>().SetTrigger("attack");
                SoundManager.PlaySound(SoundType.Attack, transform.position);
            }
        }

        public void OnGather(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            GatherPressed = true;
        }

        public void OnUse(InputAction.CallbackContext context)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
            {
                if (hit.collider.TryGetComponent<Door>(out Door door))
                {
                    if (door.IsOpen)
                    {
                        door.Close();
                    }
                    else
                    {
                        door.Open(transform.position);
                    }
                }
            }
        }
        #endregion
    }
}