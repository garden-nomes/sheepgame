using UnityEngine;

namespace SheepAI.States
{
    public class AwareState : SheepState
    {
        public float resetTime = 1f;

        private float resetTimer = 0f;

        public override void Enter(SheepState previous)
        {
            resetTimer = resetTime;
        }

        public override void Update()
        {
            sheep.isHeadDown = false;

            var toPlayer = sheep.transform.position - sheep.Awareness.player.transform.position;

            if (toPlayer.LessThan(sheep.wanderFleeRadius))
            {
                stateMachine.Push<FleePlayerState>();
            }
            else if (!toPlayer.LessThan(sheep.wanderAlertRadius))
            {
                resetTimer -= Time.deltaTime;
            }
            else if (resetTimer < resetTime)
            {
                resetTimer += Time.deltaTime;
            }

            if (resetTimer <= 0f)
            {
                stateMachine.Pop();
            }
        }
    }
}
