using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public interface IState<T>
    {
        void Enter(T a_owner);
        void Update(T a_owner);
        void Exit(T a_owner);
    }
    public class StateMachine<T>
    {
        public IState<T> currentState { get; private set; }
        public T owner;

        public StateMachine(T _owner)
        {
            owner = _owner;
            currentState = null;
        }

        public void ChangeState(IState<T> a_newState)
        {
            if (currentState != null)
            {
                if (currentState.GetType() != a_newState.GetType())
                {
                    currentState.Exit(owner);
                    currentState = a_newState;
                    currentState.Enter(owner);
                }
            }
            else
            {
                currentState = a_newState;
                currentState.Enter(owner);
            }
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.Update(owner);
            }
        }
    }
}
