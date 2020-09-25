using System.Collections.Generic;
using System.Linq;

namespace SheepAI
{
    public class SheepStateMachine
    {
        public SheepState CurrentState => stack.LastOrDefault();

        private List<SheepState> stack;
        private readonly Sheep sheep;

        public SheepStateMachine(Sheep sheep)
        {
            this.sheep = sheep;
            stack = new List<SheepState>();
        }

        public void Push<TState>() where TState : SheepState, new()
        {
            var state = new TState();
            state.Init(sheep, this);

            CurrentState?.Exit(state);
            state.Enter(CurrentState);
            stack.Add(state);
        }

        public void Pop()
        {
            if (!stack.Any())
            {
                return;
            }

            var previousState = CurrentState;
            stack.RemoveAt(stack.Count - 1);
            previousState.Exit(CurrentState);
            CurrentState.Enter(previousState);
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        public void OnInteract()
        {
            CurrentState?.OnInteract();
        }
    }
}
