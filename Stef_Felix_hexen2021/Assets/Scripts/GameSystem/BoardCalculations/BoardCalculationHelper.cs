using System.Collections.Generic;
using System.Linq;
using BoardSystem;
using GameSystem.Utils;
using GameSystem.Views;
using UnityEngine;

namespace GameSystem.BoardCalculations
{
    public class BoardCalculationHelper
    {
        private readonly Dictionary<OffsetDirection, CubeOffset> _offsetValues = new Dictionary<OffsetDirection, CubeOffset>();

        private readonly Board<HexPieceView> _board;


        public List<OffsetDirection> OffsetDirections => _offsetValues.Keys.ToList();

        public BoardCalculationHelper(Board<HexPieceView> board)
        {
            _board = board;

            _offsetValues.Add(OffsetDirection.Left, new CubeOffset(-1, 1, 0));
            _offsetValues.Add(OffsetDirection.UpLeft, new CubeOffset(0, 1, -1));
            _offsetValues.Add(OffsetDirection.DownLeft, new CubeOffset(-1, 0, 1));
            _offsetValues.Add(OffsetDirection.Right, new CubeOffset(1, -1, 0));
            _offsetValues.Add(OffsetDirection.UpRight, new CubeOffset(1, 0, -1));
            _offsetValues.Add(OffsetDirection.DownRight, new CubeOffset(0, -1, 1));
        }

        public Position Add(Tile tile, Position direction) // return position in the direction of the tile
        {
            var tilePos = tile.Position;

            var newPos = new Position
            {
                X = tilePos.X + direction.X,
                Y = tilePos.Y + direction.Y,
                Z = tilePos.Z + direction.Z
            };

            return newPos;
        }

        public Tile TileAdd(Tile tile, Position direction) => _board.TileAt(Add(tile, direction));

        public List<Tile> GetLines(Tile center, OffsetDirection direction)
        {
            List<Tile> tiles = new List<Tile>();

            _offsetValues.TryGetValue(direction, out var value);

            Tile goTile = TileAdd(center, value.OffsetPosition);

            while (goTile != null)
            {
                tiles.Add(goTile);
                goTile = TileAdd(goTile, value.OffsetPosition);
            }

            return tiles;
        }

        public Position Scale(Tile startTile, OffsetDirection direction, int scale)
        {
            _offsetValues.TryGetValue(direction, out var value);

            var newPos = new Position
            {
                X = value.OffsetPosition.X * scale,
                Y = value.OffsetPosition.Y * scale,
                Z = value.OffsetPosition.Z * scale,
            };

            return Add(startTile, newPos);
        }

        public Position GetOffset(Position position, OffsetDirection direction, int scale = 1)
        {
            _offsetValues.TryGetValue(direction, out var value);

            var newPos = new Position
            {
                X = position.X + value.OffsetPosition.X * scale,
                Y = position.Y + value.OffsetPosition.Y * scale,
                Z = position.Z + value.OffsetPosition.Z * scale,
            };

            return newPos;
        }

        public List<Tile> GetRadius(Tile centerTile, int radius)
        {
            List<Tile> tileList = new List<Tile>();
            Position goPosition = Scale(centerTile, OffsetDirection.Left, radius);

            Tile startTile = _board.TileAt(goPosition);
            if (startTile != null)
            {
                tileList.Add(startTile);
            }

            for (int i = 0; i < 6; i++) //6 = amount of hexdirections
            {
                for (int j = 0; j < radius; j++)
                {
                    goPosition = GetOffset(goPosition, (OffsetDirection)i);
                    Tile newTile = _board.TileAt(goPosition);

                    if (newTile != null)
                    {
                        tileList.Add(newTile);
                    }
                }
            }
            return tileList;
        }

        public int TrueModulo(int nr, int mod)
        {
            return (nr % mod + mod) % mod;
        }

        public Tile GetNeighbour(Tile startTile, OffsetDirection direction)
        {
            _offsetValues.TryGetValue(direction, out var value);

            return TileAdd(startTile, value.OffsetPosition);
        }

        public List<Tile> GetNeighbours(Tile startTile, bool pieceCheck = false)
        {
            var tileList = new List<Tile>();

            foreach (OffsetDirection direction in OffsetDirections)
            {
                var neighbour = GetNeighbour(startTile, direction);

                if (neighbour != null)
                {
                    var neighbourPiece = _board.PieceAt(neighbour);

                    if (!pieceCheck || pieceCheck && neighbourPiece == null)
                    {
                        tileList.Add(neighbour);
                    }
                }
            }
            return tileList;
        }

        public float GetDistance(Tile fromTile, Tile toTile)
        {
            var fromPosition = fromTile.Position;
            var toPosition = toTile.Position;

            var xDistance = Mathf.Abs(fromPosition.X - toPosition.X);
            var yDistance = Mathf.Abs(fromPosition.Y - toPosition.Y);
            var zDistance = Mathf.Abs(fromPosition.Z - toPosition.Z);

            var totalDistance = xDistance + yDistance + zDistance;

            return totalDistance / 2f;
        }


    }
}
