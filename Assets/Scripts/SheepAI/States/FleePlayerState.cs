using UnityEngine;

namespace SheepAI.States
{
    public class FleePlayerState : SheepState
    {
        Vector2 fleeTarget;

        public override void Enter(SheepState previous)
        {
            fleeTarget = sheep.Awareness.player.transform.position;
        }

        public override void Update()
        {
            sheep.isHeadDown = false;

            // if the sheep can hear player, update position
            if (sheep.Awareness.player.IsNoisy)
            {
                fleeTarget = sheep.Awareness.player.transform.position;
            }

            // flocking behaviours
            steering.FleeFrom(fleeTarget);
            steering.Seperate(
                sheep.Awareness.neighbors,
                sheep.fleeSeparationRadius,
                sheep.fleeSeparationWeight);
            steering.Cohere(sheep.Awareness.neighbors, sheep.fleeCohesionWeight);

            // if out of range revert to wander
            var toTarget = fleeTarget - sheep.Position;
            if (!toTarget.LessThan(sheep.fleeRadius + 3f))
            {
                stateMachine.Pop();
            }
        }
    }
}
