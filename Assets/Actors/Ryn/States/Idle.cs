using UnityEngine;

namespace Assets.Actors.Ryn.States
{
    public class Idle : RynState
    {
        public override void Enter(string prevState)
        {
            ryn.anim.SetState(RynAnim.STATE_FLY);
        }

        public override void Update(float deltaTime)
        {
            if (ryn.enemyTarget)
            {
                ryn.inputMotion = ryn.DirToTarget(ryn.enemyTarget);
                ryn.HandleMotion(deltaTime);
                ryn.SetLookAtTransform(ryn.enemyTarget);
                ryn.ChangeState("chase");
                return;
            }

            ryn.inputMotion = Vector3.zero;
            ryn.HandleMotion(deltaTime);
        }
    }
}
