using UnityEngine;

public class RynAnim : MonoBehaviour
{
    public const int STATE_FLY = 0;
    public const int STATE_SLASH_LR = 1;
    public const int STATE_SLASH_RL = 2;

    public Animator animator;

    private int hashState;
    private int hashMoveX;
    private int hashMoveY;
    private int hashRelSpeed;
    private int hashRelEulerSpeedX;
    private int hashRelEulerSpeedY;

    private void Awake()
    {
        hashState = Animator.StringToHash("State");
        hashMoveX = Animator.StringToHash("MoveX");
        hashMoveY = Animator.StringToHash("MoveY");
        hashRelSpeed = Animator.StringToHash("RelSpeed");
        hashRelEulerSpeedX = Animator.StringToHash("RelEulerSpeedX");
        hashRelEulerSpeedY = Animator.StringToHash("RelEulerSpeedY");
    }

    public void SetVelocity(Vector3 velocity)
    {
        Vector3 vel = Quaternion.Inverse(transform.rotation) * velocity;
        animator.SetFloat(hashMoveX, vel.x);
        animator.SetFloat(hashMoveY, vel.z);
    }

    public void SetState(int state)
    {
        animator.SetInteger(hashState, state);
    }

    public float GetRelSpeed()
    {
        return animator.GetFloat(hashRelSpeed);
    }

    public Vector3 GetRelTurnSpeed()
    {
        return new Vector3(animator.GetFloat(hashRelEulerSpeedX) * Mathf.Rad2Deg,
                            animator.GetFloat(hashRelEulerSpeedY) * Mathf.Rad2Deg,
                            0f);
    }
}
