using UnityEngine;

namespace Assets.Actors.Ryn.States
{
    public class SlashAttack : RynState
    {
        public float MAX_ATTACK_TIME = 1.5f;

        private float attackTime = 0f;

        public override void Enter(string prevState)
        {
            attackTime = 0f;

            ryn.lookEuler.x = 0f;

            ryn.anim.SetState(RynAnim.STATE_SLASH_RL);
        }

        public override void Update(float deltaTime)
        {
            Vector3 dir = ryn.enemyTarget == null ? ryn.transform.forward : ryn.DirToTarget(ryn.enemyTarget);
            ryn.HandleDirRelMotion(dir, deltaTime);

            if (attackTime > MAX_ATTACK_TIME)
            {
                ryn.ChangeState("slash_attack2");
                return;
            }

            attackTime += deltaTime;
        }
    }
}
