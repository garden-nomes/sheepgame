using UnityEngine;

namespace SheepAI.States
{
    public class PlayerPetState : SheepState
    {
        private float spawnHeartTimer = 0f;

        public override void Enter(SheepState previous)
        {
            spawnHeartTimer = 0f;
        }

        public override void Update()
        {

            sheep.isHeadDown = false;

            var player = sheep.Awareness.player;
            var animator = sheep.GetComponent<SheepAnimator>();

            var offset = player.petOffset;
            offset.x *= player.GetComponent<PlayerAnimator>().IsFacingRight ? 1 : -1;
            var target = (Vector2) player.transform.position + offset;

            steering.MoveTowards(target, .1f);

            if ((target - sheep.Position).LessThan(.2f))
            {
                sheep.transform.position = target;
            }

            if (steering.Velocity.sqrMagnitude == 0)
            {
                animator.flipX = sheep.transform.position.x < player.transform.position.x;
            }

            spawnHeartTimer += Time.deltaTime;
            if (spawnHeartTimer >= sheep.petSpawnHeartRate)
            {
                spawnHeartTimer = 0f;
                sheep.heartSpawner.ShowHearts(1);
            }

            if (!player.IsPetting)
            {
                stateMachine.Pop();
            }
        }
    }
}
