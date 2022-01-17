using UnityEngine;
using UnityEngine.UI;
using StateSystem;
using GameSystem.States;

namespace GameSystem.States
{
    class EndScreenState : GameStateBase
    {
        private StateMachine<GameStateBase> _stateMachine;
        public EndScreenState(Canvas canvas, StateMachine<GameStateBase> stateMachine)
        {
            canvasState = canvas;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            canvasState.enabled = true;


            // enable UI
        }
        public override void OnExit()
        {
            base.OnExit();

            canvasState.enabled = false;

            // disable UI
        }

        
    }
}