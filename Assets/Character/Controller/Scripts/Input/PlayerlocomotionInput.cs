using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

namespace Script.Controller
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        #region Class Variables 

        [Header("Sprint Settings")]
        [SerializeField] private bool holdToSprint = true;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 20f;
        [SerializeField] private float staminaRegenRate = 10f;

        private float staminaRegenMultiplier = 1f;
        private float currentStamina;
        private bool isSprinting;
        private HUDManager hudManager;

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintToggledOn { get; private set; }
        public bool WalkToggledOn { get; private set; }
        #endregion

        #region StartUp 

        private void Start()
        {
            currentStamina = maxStamina;
            hudManager = InstanceHandler.GetInstance<HUDManager>();
            if (hudManager != null)
            {
                hudManager.SetMaxStamina(maxStamina);
                hudManager.SetStamina(currentStamina);
            }
        }

        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }
        #endregion

        #region LateUpdate Logic
        public void LateUpdate()
        {
            JumpPressed = false;
        }

        private void Update()
        {
            HandleStamina();
        }
        #endregion

        #region Input Callbacks

        private void HandleStamina()
        {
            // Check if the player is moving
            bool isMoving = MovementInput.magnitude > 0;

            if (isSprinting && isMoving && currentStamina > 0)
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;
                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    SprintToggledOn = false; 
                }
            }
            else if ((!isSprinting || !isMoving) && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * staminaRegenMultiplier * Time.deltaTime;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
            if (hudManager != null)
            {
                hudManager.SetStamina(currentStamina);
            }
        }

        public void ApplyStaminaRegenBoost(float multiplier, float duration)
        {
            StartCoroutine(StaminaRegenBoostRoutine(multiplier, duration));
        }

        private IEnumerator StaminaRegenBoostRoutine(float multiplier, float duration)
        {
            staminaRegenMultiplier = multiplier;
            yield return new WaitForSeconds(duration);
            staminaRegenMultiplier = 1f; // Reset to normal
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (currentStamina > 0)
                {
                    SprintToggledOn = holdToSprint || !SprintToggledOn;
                    isSprinting = SprintToggledOn;
                }
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
                isSprinting = false;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            JumpPressed = true;
        }

        public void OnToggleWalk(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            WalkToggledOn = !WalkToggledOn;
        }
        #endregion
    }
}