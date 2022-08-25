
using System;
using System.Collections.Generic;
namespace IngameScript
{
    partial class Program
    {
        public class StackFSM
        {
            private Stack<Action> stack;

            public StackFSM()
            {
                stack = new Stack<Action>();
            }

            public void Update()
            {
                var currentStateFunction = getCurrentState();

                if (currentStateFunction != null)
                    currentStateFunction();

            }
            public Action PopState() =>
                stack.Pop();

            public void PushState(Action state)
            {
                if (getCurrentState() != state)
                    stack.Push(state);
            }

            public Action getCurrentState() =>
                stack.Count > 0 ? stack.Peek() : null;
        }
    }
}
