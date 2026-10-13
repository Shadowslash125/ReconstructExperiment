using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class Fall : ManState
    {
        public override void Enter(string prevState)
        {
            man.anim.SetUpperLayerWeight(1f);
            man.anim.SetMovement(ManAnim.MOVEMENT_FLY);
        }

        public override void Update(float deltaTime)
        {
            man.MotionPlatformer(deltaTime);
            if (man.inputAim)
            {
                man.TurnTowardLook(deltaTime);
            }
            else
            {
                man.TurnTowardLastMotion(deltaTime);
            }
            man.anim.SetState(ManAnim.STATE_AIM);

            if (man.IsGrounded() && man.HasMotion())
            {
                man.ChangeState("run");
                return;
            }
            if (man.IsGrounded())
            {
                man.ChangeState("idle");
            }
        }

        public override void Exit()
        {

        }
    }
}