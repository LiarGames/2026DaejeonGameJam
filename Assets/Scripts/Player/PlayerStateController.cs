using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Stunned,
    Dead
}

public class PlayerStateController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    public void ChangeState(PlayerState newState)
    {
        CurrentState = newState;
    }

    public bool CanMove()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Moving;
    }

    public bool CanAttack()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Moving;
    }
}
