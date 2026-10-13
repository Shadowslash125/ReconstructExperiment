using UnityEngine;

namespace Assets.Actors.Man.States
{
    public class Jump : ManState
    {
        private const float MAX_DELAY_FLY = 0.5f;

        private float delayFly = 0f;

        public override void Enter(string prevState)
        {
            delayFly = 0f;

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
                return;
            }

            delayFly = man.inputJump ? delayFly + deltaTime : 0f;
            if (delayFly >= MAX_DELAY_FLY)
            {
                man.ChangeState("fly");
            }
        }

        public override void Exit()
        {
            
        }
    }
}