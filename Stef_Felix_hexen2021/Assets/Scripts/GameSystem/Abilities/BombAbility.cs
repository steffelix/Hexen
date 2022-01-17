using System.Collections.Generic;
using BoardSystem;
using System;
using GameSystem.Abilities;
using GameSystem.BoardCalculations;
using GameSystem.Views;
using UnityEngine;

namespace GameSystem.Abilities
{
    [Ability("Bomb")]
    public class BombAbility : AbilityBase
    {
        private readonly Board<HexPieceView> _board;
        private readonly BoardCalculationHelper _boardCalculationHelper;


        public BombAbility(Board<HexPieceView> board)
        {
            _board = board;
            _boardCalculationHelper = new BoardCalculationHelper(board);

        }
        public override List<Tile> OnTileHold(Tile playerTile, Tile holdTile)
        {
            var tileList = _boardCalculationHelper.GetRadius(holdTile, 1);

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
                if (_board.PieceAt(fromTile) != null)
                {
                    _board.Take(fromTile);

                }

                if(fromTile != null)
                {

                    fromTile.OnTileExploded(EventArgs.Empty);
                    _board.RemoveTile(fromTile);
                }
            }


        }
    }
}
