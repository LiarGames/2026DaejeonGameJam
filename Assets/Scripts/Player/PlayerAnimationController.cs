using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private PlayerStateController stateController;
    [SerializeField] private PlayerMovement playerMovement;

    private PlayerState _lastState;

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
}