
using UnityEngine;
using UnityEngine.UI;
using StateSystem;
using GameSystem.States;
using GameSystem.Views;
using System;
using UnityEditor;

namespace GameSystem.States
{
    class StartScreenState : GameStateBase
    {
        private Button _button;
        private StateMachine<GameStateBase> _stateMachine;
        public StartScreenState(Canvas canvas, StateMachine<GameStateBase> stateMachine)
        {
            canvasState = canvas;
            _stateMachine = stateMachine;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            canvasState.enabled = true;
            _button = canvasState.GetComponentInChildren<Button>();
            _button.onClick.AddListener(OnButtonClick);


            // enable UI
        }
        public override void OnExit()
        {
            base.OnExit();

            canvasState.enabled = false;

            // disable UI
        }

        private void OnButtonClick()
        {

            _stateMachine.MoveTo("Play");
        }
    }
}
