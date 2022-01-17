using BoardSystem;
using GameSystem.States;
using StateSystem;
using UnityEngine;

namespace GameSystem.Views
{
    public class PlayerView : HexPieceView
    {
        [SerializeField] private PositionHelper _positionHelper = null;
        private Transform _boardView;
        public StateMachine<GameStateBase> PlayerStateMachine;

        private void Start()
        {
            _boardView = FindObjectOfType<BoardView>().transform;
        }
        public override void Moved(Tile fromTile, Tile toTile)
        {
            transform.position = _positionHelper.ToWorldPosition(_boardView, toTile.Position);
        }
        public override void Taken() 
        {
            Destroy(gameObject);
            Debug.Log("player tasks");
            PlayerStateMachine.MoveTo("End");
        }

    }
}
