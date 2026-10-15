using UnityEngine;

namespace Assets.Actors.Ryn.States
{
    public class Chase : RynState
    {
        private const float MELEE_RADIUS = 10f;

        public override void Enter(string prevState)
        {
            ryn.anim.SetState(RynAnim.STATE_FLY);
        }

        public override void Update(float deltaTime)
        {
            Transform tar = ryn.enemyTarget;
            if (tar == null)
            {
                ryn.inputMotion = Vector3.zero;
                ryn.HandleMotion(deltaTime);
                ryn.ChangeState("idle");
                return;
            }

            ryn.inputMotion = ryn.DirToTarget(tar);
            ryn.HandleMotion(deltaTime);
            ryn.SetLookAtTransform(tar);

            if (ryn.DistToTarget(tar) <= MELEE_RADIUS)
            {
                ryn.ChangeState("slash_attack");
            }
        }
    }
}
