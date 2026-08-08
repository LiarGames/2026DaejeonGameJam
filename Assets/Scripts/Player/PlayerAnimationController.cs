using System.Collections;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private PlayerStateController stateController;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Damage Flash")]
    [Min(0f)]
    [SerializeField] private float damageFlashDuration = 0.3f;
    [SerializeField] private Color damageFlashColor = Color.red;

    private PlayerState _lastState;
    private Coroutine _damageFlashCoroutine;
    private Color _baseSkeletonColor;
    private bool _hasBaseSkeletonColor;

    private void Update()
    {
        if (_lastState == stateController.CurrentState)
            return;

        _lastState = stateController.CurrentState;
        PlayStateAnimation(_lastState);

        UpdateFacingDirection();
    }

    private void PlayStateAnimation(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                skeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "animation",
                    true
                );
                break;

            case PlayerState.Moving:
                skeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "걷기",
                    true
                );
                break;

            case PlayerState.Attacking:
            case PlayerState.Dashing:
                skeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "공격",
                    false
                );
                break;
        }
    }

    private void UpdateFacingDirection()
    {
        float x = playerMovement.LastMoveDirection.x;
        if (Mathf.Abs(x) < 0.01f)
        {
            return;
        }

        skeletonAnimation.Skeleton.ScaleX = x < 0f ? 1f : -1f;
    }

    public void PlayDamageFlash()
    {
        if (skeletonAnimation == null ||
            skeletonAnimation.Skeleton == null)
            return;

        if (!_hasBaseSkeletonColor)
        {
            _baseSkeletonColor =
                skeletonAnimation.Skeleton.GetColor();
            _hasBaseSkeletonColor = true;
        }

        if (_damageFlashCoroutine != null)
        {
            StopCoroutine(_damageFlashCoroutine);
            skeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
        }

        _damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < damageFlashDuration)
        {
            float progress = damageFlashDuration > 0f
                ? elapsedTime / damageFlashDuration
                : 1f;
            float redAmount = Mathf.Sin(progress * Mathf.PI);
            Color color = Color.Lerp(
                _baseSkeletonColor,
                damageFlashColor,
                redAmount
            );

            skeletonAnimation.Skeleton.SetColor(color);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        skeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
        _damageFlashCoroutine = null;
    }

    private void OnDisable()
    {
        if (_hasBaseSkeletonColor &&
            skeletonAnimation != null &&
            skeletonAnimation.Skeleton != null)
        {
            skeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
        }

        _damageFlashCoroutine = null;
    }
}
