using System.Collections.Generic;
using BoardSystem;
using GameSystem.Abilities;
using GameSystem.BoardCalculations;
using GameSystem.Views;

namespace GameSystem.Abilities
{
    [Ability("Tornado")]

    public class TornadoAbility : AbilityBase
    {
        private readonly Board<HexPieceView> _board;
        private readonly BoardCalculationHelper _boardCalculationHelper;

        public TornadoAbility(Board<HexPieceView> board)
        {
            _board = board;
            _boardCalculationHelper = new BoardCalculationHelper(board);

        }

        public override List<Tile> OnTileHold(Tile playerTile, Tile holdTile)
        {
            var tileList = _boardCalculationHelper.GetRadius(holdTile, 2);

            if (_board.PieceAt(holdTile) == null)
            {
                tileList.Add(holdTile);
            }

            return tileList;
        }
        public override void OnTileRelease(Tile playerTile, Tile holdTile)
        {

            var tileList = OnTileHold(playerTile, holdTile);
            if (!tileList.Contains(holdTile)) return;


            foreach (var fromTile in tileList)
            {
                if(_board.PieceAt(fromTile) is EnemyView)
                {
                    _board.Take(fromTile);

                }
            }

            _board.Move(playerTile, holdTile);
            if (!OnTileHold(playerTile, holdTile).Contains(holdTile)) return;

        }
    }

}