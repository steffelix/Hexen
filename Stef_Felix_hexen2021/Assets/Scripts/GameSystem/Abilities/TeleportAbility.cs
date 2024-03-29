using System.Collections.Generic;
using BoardSystem;
using GameSystem.Abilities;
using GameSystem.BoardCalculations;
using GameSystem.Views;

namespace GameSystem.Abilities
{
    [Ability("Teleport")]
    public class TeleportAbility : AbilityBase
    {
        private readonly Board<HexPieceView> _board;

        public TeleportAbility(Board<HexPieceView> board)
        {
            _board = board;
        }
        public override List<Tile> OnTileHold(Tile playerTile, Tile holdTile)
        {
            List<Tile> tileList = new List<Tile>();

            if (_board.PieceAt(holdTile) == null)
            {
                tileList.Add(holdTile);
            }
            return tileList;
        }

        public override void OnTileRelease(Tile playerTile, Tile holdTile)
        {
            if (!OnTileHold(playerTile, holdTile).Contains(holdTile)) return;
            _board.Move(playerTile, holdTile);
        }
    }
}
