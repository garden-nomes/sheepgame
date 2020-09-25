using System.Linq;
using UnityEngine;

namespace SheepAI.States
{
    public class WanderState : SheepState
    {
        private Vector2 wanderTarget;
        private float greetResetTimer = 0f;

        public override void Enter(SheepState previous)
        {
            wanderTarget = sheep.Position;

            if (previous is GreetPlayerState)
            {
                greetResetTimer = sheep.greetResetTime;
            }
        }

        public override void Update()
        {
            if (greetResetTimer > 0f) greetResetTimer -= Time.deltaTime;

            var toPlayer = (sheep.transform.position - sheep.Awareness.player.transform.position);

            // check if apple within range
            if (sheep.Awareness.apples.Any())
            {
                var closestApple = sheep.Awareness.apples
                    .OrderBy(apple => (sheep.transform.position - apple.transform.position).sqrMagnitude)
                    .FirstOrDefault();

                // seek apple
                wanderTarget = closestApple.transform.position;

                // pick up apple if in range
                if ((closestApple.transform.position - sheep.transform.position).LessThan(sheep.pickupRadius))
                {
                    GameObject.Destroy(closestApple);
                    sheep.ReceiveApple();
                }
            }
            else
            {
                // change target every so often
                var changeTargetChance = sheep.IsOnPasture ?
                    sheep.grazeChangeTarget :
                    sheep.wanderChangeTarget;

                if (Random.value < changeTargetChance * Time.deltaTime)
                {
                    wanderTarget = PickNewWanderPoint();
                }
            }

            steering.MoveTowards(wanderTarget);

            // graze when on pasture and not moving
            if (steering.Velocity.sqrMagnitude == 0 && sheep.IsOnPasture)
            {
                sheep.isHeadDown = true;
                sheep.pasture.exhaustion -= Time.deltaTime;
            }
            else
            {
                sheep.isHeadDown = false;
            }

            if (sheep.attitude < 1f)
            {
                // go alert when player approaches
                if (toPlayer.LessThan(sheep.wanderAlertRadius))
                {
                    stateMachine.Push<AwareState>();
                }
            }
            else if (sheep.attitude >= 2f)
            {
                // green when player approaches
                if (greetResetTimer <= 0f && toPlayer.LessThan(sheep.greetRadius))
                {
                    stateMachine.Push<GreetPlayerState>();
                }
            }
        }

        Vector2 PickNewWanderPoint()
        {
            var radius = sheep.IsOnPasture ? sheep.grazeRadius : sheep.wanderRadius;

            for (int i = 0; i < 32; i++)
            {
                var sample = sheep.Position + Random.insideUnitCircle * radius;

                if (CanMoveTo(sample) && (i > 8 || IsPointOnPasture(sample)))
                {
                    return sample;
                }
            }

            return sheep.Position;
        }

        bool IsPointOnPasture(Vector2 point)
        {
            return Physics2D.OverlapPointAll(point)
                .Any(collider =>
                {
                    var pasture = collider.GetComponent<Pasture>();
                    return pasture != null && !pasture.isExhausted;
                });
        }

        bool CanMoveTo(Vector2 point)
        {
            var toTarget = point - sheep.Position;

            var filter = new ContactFilter2D();
            filter.SetLayerMask(sheep.blockingLayer);

            var raycast = sheep.GetComponent<Collider2D>()
                .Cast(toTarget, filter, new RaycastHit2D[1], toTarget.magnitude);

            return raycast == 0;
        }
    }
}
