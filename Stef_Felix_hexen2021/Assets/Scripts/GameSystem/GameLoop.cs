using System.Collections;
using System;
using GameSystem.Views;
using UnityEngine;
using Utils;
using BoardSystem;
using StateSystem;
using GameSystem.States;
using GameSystem.Abilities;
using AbilitySystem;


namespace GameSystem
{
    public class GameLoop : SingletonMonoBehaviour<GameLoop>
    {
        [SerializeField] private PositionHelper _positionHelper = null;

        private BoardView _boardView;
        private PlayerView _playerView;
        private StateMachine<GameStateBase> _gameStateMachine;


        public event EventHandler Initialized;

        public Board<HexPieceView> Board = new Board<HexPieceView>(3);
        public Deck<AbilityBase> Deck { get; private set; }
        public ActiveHand<AbilityBase> ActiveHand { get; set; }

        private void Start()
        {
            Deck = new Deck<AbilityBase>();
            AddAbilities();

            ActiveHand = Deck.CreateActiveHand(5);
            _boardView = FindObjectOfType<BoardView>();

            ConnectPlayer();
            ConnectEnemies();

            ConnectGameStates();

            OnInitialized(EventArgs.Empty);
        }

        
        protected virtual void OnInitialized(EventArgs arg)
        {
            EventHandler handler = Initialized;
            handler?.Invoke(this, arg);
        }
        private void AddAbilities()
        {
            Deck.AddAbilityAction("ForwardAttack", new ForwardAttackAbility(Board));
            Deck.AddAbilityAction("SwingAttack", new SwingAttackAbility(Board));
            Deck.AddAbilityAction("Teleport", new TeleportAbility(Board));
            Deck.AddAbilityAction("Knockback", new KnockbackAbility(Board));
            
            Deck.AddAbility("ForwardAttack", 3);
            Deck.AddAbility("SwingAttack", 3);
            Deck.AddAbility("Teleport", 3);
            Deck.AddAbility("Knockback", 3);
        }

        internal void OnEnterTile(Tile holdTile) => _gameStateMachine.CurrentState.OnEnterTile(holdTile);
        internal void OnExitTile(Tile holdTile) => _gameStateMachine.CurrentState.OnExitTile(holdTile);
        internal void OnAbilityBeginDrag(string ability) => _gameStateMachine.CurrentState.OnAbilityBeginDrag(ability);
        internal void OnAbilityReleased(Tile holdTile) => _gameStateMachine.CurrentState.OnAbilityReleased(holdTile);
        internal void OnAbilityReleasedEmpty() => _gameStateMachine.CurrentState.OnAbilityReleasedEmpty();

        private void ConnectPlayer()
        {
            _playerView = FindObjectOfType<PlayerView>();
            Board.Place(Board.TileAt(_positionHelper.ToBoardPosition(_boardView.transform, _playerView.transform.position)), _playerView);
        }
        private void ConnectEnemies()
        {
            foreach (var enemyView in FindObjectsOfType<EnemyView>())
            {
                Board.Place(Board.TileAt(_positionHelper.ToBoardPosition(_boardView.transform, enemyView.transform.position)), enemyView);
            }
        }

        private void ConnectGameStates()
        {
            _gameStateMachine = new StateMachine<GameStateBase>();

            var playerTurnState = new PlayerTurnState(Board, Deck, ActiveHand, _playerView);
            _gameStateMachine.RegisterState(GameStates.Player, playerTurnState);
            _gameStateMachine.SetStartState(GameStates.Player);

            //var enemyTurnState = new EnemyTurnState(Board, _playerView);
            //_gameStateMachine.RegisterState(GameStates.Enemy, enemyTurnState);
            //_gameStateMachine.SetStartState(GameStates.Enemy);
        }

    }
}
