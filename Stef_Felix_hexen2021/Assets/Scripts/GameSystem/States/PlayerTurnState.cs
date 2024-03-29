using System.Collections.Generic;
using AbilitySystem;
using BoardSystem;
using GameSystem.Abilities;
using GameSystem.BoardCalculations;
using GameSystem.Views;
using StateSystem;
using UnityEngine;

namespace GameSystem.States
{
    public class PlayerTurnState : GameStateBase
    {
        private readonly PlayerView _player;
        private readonly Board<HexPieceView> _board;
        private readonly Deck<AbilityBase> _deck;
        private readonly ActiveHand<AbilityBase> _activeHand;
        private readonly BoardCalculationHelper _boardCalculationHelper;

        private List<Tile> _validTiles = new List<Tile>();
        private AbilityBase _draggedAbility;
        private string _ability;
        private int _amountOfAbilitiesUsed;

        private StateMachine<GameStateBase> _stateMachine;

        public PlayerTurnState(Board<HexPieceView> board, Deck<AbilityBase> pile, ActiveHand<AbilityBase> activeHand, PlayerView player, Canvas canvas, StateMachine<GameStateBase> stateMachine)
        {
            _board = board;
            _deck = pile;
            _activeHand = activeHand;
            _player = player;
            _boardCalculationHelper = new BoardCalculationHelper(board);
            canvasState = canvas;
            _stateMachine = stateMachine;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _amountOfAbilitiesUsed = 0;

            canvasState.enabled = true;

            // enable UI
        }
        public override void OnExit()
        {
            base.OnExit();

            canvasState.enabled = false;

            // disable UI
        }

        public override void OnEnterTile(Tile holdTile)
        {
            if (_draggedAbility != null)
            {
                _validTiles = _draggedAbility.OnTileHold(_board.TileOf(_player), holdTile);

                _board.Highlight(_validTiles);
            }
            else return;
            
        }

        public override void OnExitTile(Tile holdTile)
        {
            _board.UnHighlight(_validTiles);
            _validTiles.Clear();
        }

        public override void OnAbilityBeginDrag(string ability)
        {

            _ability = ability;
            _draggedAbility = _deck.GetAbilityAction(ability);
        }

        public override void OnAbilityReleasedEmpty()
        {
            _draggedAbility = null;

        }

        public override void OnAbilityReleased(Tile holdTile)
        {
            if (_draggedAbility == null) 
            {
                return;
                
            }

            _board.UnHighlight(_validTiles);


            if (!_validTiles.Contains(holdTile))
            {

                _draggedAbility = null;

            }
            else
            {
                _draggedAbility.OnTileRelease(_board.TileOf(_player), holdTile);
                _activeHand.RemoveAbility(_ability);
                _activeHand.InitializeActiveHand();
                _draggedAbility = null;
                _ability = null;
                ++_amountOfAbilitiesUsed;
            }
            _validTiles.Clear();
            if (_amountOfAbilitiesUsed <= 1) return;

        }
    }
}