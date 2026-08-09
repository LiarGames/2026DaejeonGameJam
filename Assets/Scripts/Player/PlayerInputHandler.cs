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
        // 도메인 리로드 등으로 Awake 없이 호출될 수 있으므로 방어한다.
        if (_input == null)
            return;

        _input.Enable();
    }

    private void OnDisable()
    {
        if (_input == null)
            return;

        _input.Disable();
    }

    private void OnDestroy()
    {
        if (_input == null)
            return;

        _input.Player.RemoveCallbacks(this);
        _input.Dispose();
    }

    private void Update()
    {
        // ESC로 일시정지 토글. (timeScale 0에서도 입력/Update는 동작)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();

        _movement.SetMovementInput(movementInput);
    }
}
