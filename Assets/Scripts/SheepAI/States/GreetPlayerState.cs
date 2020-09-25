namespace SheepAI.States
{
    public class GreetPlayerState : SheepState
    {
        public override void Update()
        {
            sheep.isHeadDown = false;

            var toPlayer = sheep.transform.position - sheep.Awareness.player.transform.position;

            if (toPlayer.LessThan(2f))
            {
                // boop
                sheep.heartSpawner.ShowHearts(1);
                EventBus.Instance.Notify(Event.SHEEP_GREET, sheep.gameObject);
                stateMachine.Pop();
            }
            else
            {
                steering.MoveTowards(sheep.Awareness.player.transform.position);
            }
        }
    }
}
