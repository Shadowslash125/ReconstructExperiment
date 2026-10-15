using UnityEngine;

namespace Assets.Actors.Ryn.States
{
    public class SlashAttack2 : RynState
    {
        public float MAX_ATTACK_TIME = 1.8f;

        private float attackTime = 0f;

        public override void Enter(string prevState)
        {
            attackTime = 0f;

            ryn.lookEuler.x = 0f;

            ryn.anim.SetState(RynAnim.STATE_SLASH_LR);
        }

        public override void Update(float deltaTime)
        {
            Vector3 dir = ryn.enemyTarget == null ? ryn.transform.forward : ryn.DirToTarget(ryn.enemyTarget);
            ryn.HandleDirRelMotion(dir, deltaTime);

            if (attackTime > MAX_ATTACK_TIME)
            {
                ryn.ChangeState("chase");
                return;
            }

            attackTime += deltaTime;
        }
    }
}
