using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, PlayerInput.IPlayerActions
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerSkillController _skills;

    private PlayerInput _input;
    private Vector2 _pointerPosition;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Player.AddCallbacks(this);
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        _input.Player.RemoveCallbacks(this);
        _input.Dispose();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();

        _movement.SetMovementInput(movementInput);
    }
}
