using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class Fly : ManState
    {
        public override void Enter(string prevState)
        {
            man.curMovementData = man.flyMoveData;

            man.anim.SetUpperLayerWeight(1f);
            man.anim.SetState(ManAnim.STATE_AIM);
        }

        public override void Update(float deltaTime)
        {
            man.MotionFlyGround(deltaTime);

            if (man.IsGrounded())
            {
                man.anim.SetMovement(ManAnim.MOVEMENT_FLY_GROUND);
            }
            else
            {
                man.anim.SetMovement(ManAnim.MOVEMENT_FLY);
            }

            if (man.inputAim)
            {
                man.TurnTowardLook(deltaTime);
            }
            else
            {
                man.TurnTowardLastMotion(deltaTime);
            }

            if (man.inputSprint && man.RequestFlyDash())
            {
                man.ChangeState("fly_dash");
            }
        }
    }
}
