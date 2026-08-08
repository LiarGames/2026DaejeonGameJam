using System.Collections;
using UnityEngine;

public class DashRunner : MonoBehaviour
{
    private Coroutine _dashCoroutine;
    private Rigidbody2D _activeRigidbody;
    private PlayerStateController _stateController;
    private PlayerState _previousState;

    public bool IsDashing { get; private set; }

    private void Awake()
    {
        _stateController = GetComponent<PlayerStateController>();
    }

    public void StartDash(
        Rigidbody2D rb,
        Vector2 direction,
        float dashSpeed,
        float dashDuration)
    {
        if (IsDashing || rb == null || direction.sqrMagnitude <= 0.001f)
            return;

        _dashCoroutine = StartCoroutine(DashRoutine(
            rb,
            direction.normalized,
            Mathf.Max(0f, dashSpeed),
            Mathf.Max(0f, dashDuration)
        ));
    }

    private IEnumerator DashRoutine(
        Rigidbody2D rb,
        Vector2 direction,
        float dashSpeed,
        float dashDuration)
    {
        IsDashing = true;
        _activeRigidbody = rb;

        if (_stateController != null)
        {
            _previousState = _stateController.CurrentState;
            _stateController.ChangeState(PlayerState.Dashing);
        }

        float elapsedTime = 0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (elapsedTime < dashDuration)
        {
            rb.linearVelocity = direction * dashSpeed;

            yield return waitForFixedUpdate;
            elapsedTime += Time.fixedDeltaTime;
        }

        FinishDash();
    }

    private void OnDisable()
    {
        if (_dashCoroutine != null)
            StopCoroutine(_dashCoroutine);

        FinishDash();
    }

    private void FinishDash()
    {
        if (_activeRigidbody != null)
            _activeRigidbody.linearVelocity = Vector2.zero;

        if (_stateController != null &&
            _stateController.CurrentState == PlayerState.Dashing)
        {
            _stateController.ChangeState(_previousState);
        }

        _activeRigidbody = null;
        _dashCoroutine = null;
        IsDashing = false;
    }
}
