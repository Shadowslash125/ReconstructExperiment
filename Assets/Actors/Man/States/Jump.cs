using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class Jump : ManState
    {
        public override void Enter(string prevState)
        {
            man.velocity.y = man.onFootMoveData.jumpForce;

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

            if (man.velocity.y < 0.2f)
            {
                man.ChangeState("fall");
            }
        }

        public override void Exit()
        {
            
        }
    }
}