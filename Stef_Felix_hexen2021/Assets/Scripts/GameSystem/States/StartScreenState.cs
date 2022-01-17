using System.Collections.Generic;
using AbilitySystem;
using BoardSystem;
using GameSystem.Abilities;
using GameSystem.BoardCalculations;
using GameSystem.Views;
using UnityEngine;

namespace GameSystem.States
{
    class StartScreenState : GameStateBase
    {
        public StartScreenState(Canvas canvas)
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
