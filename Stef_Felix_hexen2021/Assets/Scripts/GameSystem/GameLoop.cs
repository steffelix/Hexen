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

        [SerializeField] private GameObject _enemyPrefab;

        [SerializeField] private Canvas _startCanvas;
        [SerializeField] private Canvas _playCanvas;
        [SerializeField] private Canvas _endCanvas;

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
            ConnectPlayer();

            OnInitialized(EventArgs.Empty);

            // tried to fix the bug where the tiles don't select properly on Unity Restart,
            // these methods resulted in more errors :(

            //RemoveBoard();
            //CreateBoard();

        }

        
        protected virtual void OnInitialized(EventArgs arg)
        {
            EventHandler handler = Initialized;
            handler?.Invoke(this, arg);
        }

        private void CreateBoard()
        {
            var tileViewFactory = _boardView.TileViewFactory;
            Debug.Assert(_boardView != null, nameof(_boardView) + " != null");

            foreach (var tile in Board.Tiles)
            {
                tileViewFactory?.CreateTileView(tile, _boardView.transform);
            }
        }

        private static void RemoveBoard()
        {
            var tiles = FindObjectsOfType<TileView>();

            foreach (TileView t in tiles)
            {
                DestroyImmediate(t.gameObject);
            }
        }

        private void AddAbilities()
        {
            Deck.AddAbilityAction("ForwardAttack", new ForwardAttackAbility(Board));
            Deck.AddAbilityAction("SwingAttack", new SwingAttackAbility(Board));
            Deck.AddAbilityAction("Teleport", new TeleportAbility(Board));
            Deck.AddAbilityAction("Knockback", new KnockbackAbility(Board));
            Deck.AddAbilityAction("Bomb", new BombAbility(Board));

            Deck.AddAbility("ForwardAttack", 3);
            Deck.AddAbility("SwingAttack", 3);
            Deck.AddAbility("Teleport", 3);
            Deck.AddAbility("Knockback", 3);
            Deck.AddAbility("Bomb", 3);
        }

        
        internal void OnEnterTile(Tile holdTile) => _gameStateMachine.CurrentState.OnEnterTile(holdTile);
        internal void OnExitTile(Tile holdTile) => _gameStateMachine.CurrentState.OnExitTile(holdTile);
        internal void OnAbilityBeginDrag(string ability) => _gameStateMachine.CurrentState.OnAbilityBeginDrag(ability);
        internal void OnAbilityReleased(Tile holdTile) => _gameStateMachine.CurrentState.OnAbilityReleased(holdTile);
        internal void OnAbilityReleasedEmpty() => _gameStateMachine.CurrentState.OnAbilityReleasedEmpty();


        private void ConnectPlayer()
        {
            _playerView = FindObjectOfType<PlayerView>();
            _playerView.PlayerStateMachine = _gameStateMachine;
            Board.Place(Board.TileAt(_positionHelper.ToBoardPosition(_boardView.transform, _playerView.transform.position)), _playerView);
        }
        private void ConnectEnemies()
        {
            foreach (var enemyView in FindObjectsOfType<EnemyView>())
            {
                Board.Place(Board.TileAt(_positionHelper.ToBoardPosition(_boardView.transform, enemyView.transform.position)), enemyView);
            }
        }

        public void SpawnEnemy(Tile tile)
        {
            var enemy = Instantiate(_enemyPrefab, _positionHelper.ToLocalPosition(tile.Position), Quaternion.identity);

            enemy.transform.position = transform.TransformPoint(enemy.transform.position);

            //places already so that no enemies will spawn on top of eachother
            Board.Place(tile, enemy.GetComponent<EnemyView>());

        }

        private void ConnectGameStates()
        {
            _gameStateMachine = new StateMachine<GameStateBase>();

            var startScreenState = new StartScreenState(_startCanvas, _gameStateMachine);
            var playerTurnState = new PlayerTurnState(Board, Deck, ActiveHand, _playerView, _playCanvas, _gameStateMachine);
            var endScreenState = new EndScreenState(_endCanvas, _gameStateMachine);


            _gameStateMachine.RegisterState(GameStates.Start, startScreenState);
            _gameStateMachine.RegisterState(GameStates.Player, playerTurnState);
            _gameStateMachine.RegisterState(GameStates.End, endScreenState);

            _gameStateMachine.SetStartState(GameStates.Start);
        }

    }
}
