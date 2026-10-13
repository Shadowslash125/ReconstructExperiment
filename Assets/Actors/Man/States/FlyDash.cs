using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class FlyDash : ManState
    {
        private float dashTime = 0f;

        public override void Enter(string prevState)
        {
            dashTime = 0f;
        }

        public override void Update(float deltaTime)
        {
            man.MotionLinearH();

            if (man.IsGrounded())
                man.anim.SetMovement(ManAnim.MOVEMENT_FLY_GROUND);
            else
                man.anim.SetMovement(ManAnim.MOVEMENT_FLY);

            if (man.inputAim)
                man.TurnTowardLook(deltaTime);
            else
                man.TurnTowardLastMotion(deltaTime);

            if (dashTime >= man.curMovementData.maxFlyDashTime)
            {
                man.ChangeState("fly");
                return;
            }

            dashTime += deltaTime;
        }
    }
}
