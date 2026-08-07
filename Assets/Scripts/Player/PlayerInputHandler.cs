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

    public void OnPointerPosition(InputAction.CallbackContext context)
    {
        _pointerPosition = context.ReadValue<Vector2>();
    }

    public void OnMoveClick(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Vector3 worldPosition =
            _mainCamera.ScreenToWorldPoint(_pointerPosition);

        Debug.Log(worldPosition);

        _movement.SetMoveTarget(worldPosition);
    }

    public void OnSkill1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(_pointerPosition);
        _skills.UseSkill(0, mouseWorldPosition);
    }

    public void OnSkill2(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(_pointerPosition);
        _skills.UseSkill(1, mouseWorldPosition);
    }

    public void OnSkill3(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(_pointerPosition);
        _skills.UseSkill(2, mouseWorldPosition);
    }

    public void OnSkill4(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(_pointerPosition);
        _skills.UseSkill(3, mouseWorldPosition);
    }
}
