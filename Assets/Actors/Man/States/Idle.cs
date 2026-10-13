using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class Idle : ManState
    {
        public override void Enter(string prevState)
        {
            man.anim.SetMovement(ManAnim.MOVEMENT_ON_FOOT);
        }

        public override void Update(float deltaTime)
        {
            man.MotionPlatformer(deltaTime);
            if (man.inputAim)
            {
                man.TurnTowardLook(deltaTime);
                man.anim.SetState(ManAnim.STATE_AIM);
                man.anim.SetUpperLayerWeight(1f);
            }
            else
            {
                man.TurnTowardLastMotion(deltaTime);
                man.anim.SetState(ManAnim.STATE_IDLE);
                man.anim.SetUpperLayerWeight(0f);
            }

            if (man.HasMotion())
            {
                man.ChangeState("run");
                return;
            }
            if (man.inputJump)
            {
                man.ChangeState("jump");
                return;
            }
            if (!man.IsGrounded())
            {
                man.ChangeState("fall");
            }
        }
    }
}
