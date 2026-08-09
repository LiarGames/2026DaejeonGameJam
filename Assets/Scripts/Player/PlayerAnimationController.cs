using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Visuals")]
    [FormerlySerializedAs("skeletonAnimation")]
    [SerializeField] private SkeletonAnimation aliveSkeletonAnimation;
    [SerializeField] private SkeletonAnimation deathSkeletonAnimation;
    [SerializeField] private PlayerStateController stateController;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Animation Names")]
    [SerializeField] private string deathAnimationName = "죽음";

    [Header("Damage Flash")]
    [Min(0f)]
    [SerializeField] private float damageFlashDuration = 0.3f;
    [SerializeField] private Color damageFlashColor = Color.red;

    private PlayerState _lastState = (PlayerState)(-1);
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
        if (state == PlayerState.Dead)
        {
            PlayDeathAnimation();
            return;
        }

        SetAliveVisualActive();

        if (aliveSkeletonAnimation == null)
            return;

        switch (state)
        {
            case PlayerState.Idle:
                aliveSkeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "animation",
                    true
                );
                break;

            case PlayerState.Moving:
                aliveSkeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "걷기",
                    true
                );
                break;

            case PlayerState.Attacking:
            case PlayerState.Dashing:
                aliveSkeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "공격",
                    false
                );
                break;

        }
    }

    private void PlayDeathAnimation()
    {
        RestoreAliveColor();

        if (deathSkeletonAnimation == null)
            return;

        float facingScale = 1f;

        if (aliveSkeletonAnimation != null &&
            aliveSkeletonAnimation.Skeleton != null)
        {
            facingScale = aliveSkeletonAnimation.Skeleton.ScaleX;
        }

        deathSkeletonAnimation.gameObject.SetActive(true);
        deathSkeletonAnimation.Initialize(false);

        if (deathSkeletonAnimation.Skeleton != null)
            deathSkeletonAnimation.Skeleton.ScaleX = facingScale;

        deathSkeletonAnimation.AnimationState.SetAnimation(
            0,
            deathAnimationName,
            false
        );

        if (aliveSkeletonAnimation != null &&
            aliveSkeletonAnimation.gameObject !=
            deathSkeletonAnimation.gameObject)
        {
            aliveSkeletonAnimation.gameObject.SetActive(false);
        }
    }

    private void SetAliveVisualActive()
    {
        if (aliveSkeletonAnimation != null)
            aliveSkeletonAnimation.gameObject.SetActive(true);

        if (deathSkeletonAnimation != null &&
            deathSkeletonAnimation.gameObject !=
            aliveSkeletonAnimation?.gameObject)
        {
            deathSkeletonAnimation.gameObject.SetActive(false);
        }
    }

    public float DeathAnimationDuration
    {
        get
        {
            if (deathSkeletonAnimation == null)
                return 0f;

            deathSkeletonAnimation.Initialize(false);

            if (deathSkeletonAnimation.Skeleton == null)
                return 0f;

            Spine.Animation animation =
                deathSkeletonAnimation.Skeleton.Data.FindAnimation(
                    deathAnimationName
                );

            return animation != null ? animation.Duration : 0f;
        }
    }

    private void UpdateFacingDirection()
    {
        float x = playerMovement.LastMoveDirection.x;
        if (Mathf.Abs(x) < 0.01f)
        {
            return;
        }

        SkeletonAnimation activeSkeleton =
            _lastState == PlayerState.Dead
                ? deathSkeletonAnimation
                : aliveSkeletonAnimation;

        if (activeSkeleton != null && activeSkeleton.Skeleton != null)
            activeSkeleton.Skeleton.ScaleX = x < 0f ? 1f : -1f;
    }

    public void PlayDamageFlash()
    {
        if (aliveSkeletonAnimation == null ||
            aliveSkeletonAnimation.Skeleton == null)
            return;

        if (!_hasBaseSkeletonColor)
        {
            _baseSkeletonColor =
                aliveSkeletonAnimation.Skeleton.GetColor();
            _hasBaseSkeletonColor = true;
        }

        if (_damageFlashCoroutine != null)
        {
            StopCoroutine(_damageFlashCoroutine);
            aliveSkeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
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

            aliveSkeletonAnimation.Skeleton.SetColor(color);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        aliveSkeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
        _damageFlashCoroutine = null;
    }

    private void RestoreAliveColor()
    {
        if (_damageFlashCoroutine != null)
            StopCoroutine(_damageFlashCoroutine);

        if (_hasBaseSkeletonColor &&
            aliveSkeletonAnimation != null &&
            aliveSkeletonAnimation.Skeleton != null)
        {
            aliveSkeletonAnimation.Skeleton.SetColor(_baseSkeletonColor);
        }

        _damageFlashCoroutine = null;
    }

    private void OnDisable()
    {
        RestoreAliveColor();
    }
}
