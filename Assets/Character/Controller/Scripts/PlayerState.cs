using UnityEngine;

namespace Script.Controller
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;
        private Animator _animator;
        private PlayerState _playerState;
        private PlayerActionsInput _playerActionsInput;


        private void Awake()
        {
            _playerState = GetComponent<PlayerState>();
            _animator = GetComponent<Animator>();
            _playerActionsInput = GetComponent<PlayerActionsInput>();
        }

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            if (_animator != null)
            {
                _animator.SetInteger("MovementState", (int)CurrentPlayerMovementState);
            }
        }

        public bool InGroundedState()
        {
            return IsStateGroundedState(CurrentPlayerMovementState);
        }

        public bool IsStateGroundedState(PlayerMovementState movementState)
        {
            return movementState == PlayerMovementState.Idling ||
                   movementState == PlayerMovementState.Walking ||
                   movementState == PlayerMovementState.Running ||
                   movementState == PlayerMovementState.Sprinting;
        }
    }

    public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3,
        Jumping = 4,
        Falling = 5,
        Strafing = 6,
        Attacking = 7, 
    }
}
