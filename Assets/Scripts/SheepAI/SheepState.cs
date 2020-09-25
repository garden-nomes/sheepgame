namespace SheepAI
{
    public abstract class SheepState
    {
        protected Sheep sheep;
        protected SteeredObject steering;
        protected SheepStateMachine stateMachine;

        public void Init(Sheep sheep, SheepStateMachine stateMachine)
        {
            this.sheep = sheep;
            this.steering = sheep.GetComponent<SteeredObject>();
            this.stateMachine = stateMachine;
        }

        public virtual void Enter(SheepState previousState) { }
        public virtual void Update() { }
        public virtual void OnInteract() { }
        public virtual void Exit(SheepState newState) { }
    }
}
