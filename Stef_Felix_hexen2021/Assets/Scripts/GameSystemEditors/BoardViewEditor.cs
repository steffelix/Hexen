using GameSystem;
using GameSystem.Views;
using UnityEngine;
using UnityEditor;
using System;
using Random = System.Random;
//using Debug = System.Diagnostics.Debug;

#if UNITY_EDITOR

namespace BoardSystem.Editor
{
    [CustomEditor(typeof(BoardView))]


    public class BoardViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var boardView = target as BoardView;
            var tileViewFactorySp = serializedObject.FindProperty("_tileViewFactory");
            var tileViewFactory = tileViewFactorySp.objectReferenceValue as TileViewFactory;
            var game = GameLoop.Instance;
            var board = game.Board;
            var random = new Random();

            if (GUILayout.Button("Remove Hex Board"))
            {
                // remove board
                var tiles = FindObjectsOfType<TileView>();

                foreach(TileView t in tiles)
                {
                    DestroyImmediate(t.gameObject);
                }
            }

            if (GUILayout.Button("Generate Hex Board"))
            {
                // generate board
                Debug.Assert(boardView != null, nameof(boardView) + " != null");

                foreach (var tile in board.Tiles)
                {
                    tileViewFactory?.CreateTileView(tile, boardView.transform);
                }
            }

            if(GUILayout.Button("Spawn Enemy"))
            {
                //spawn enemy
                for (int i = 0; i < boardView.EnemyAmount; i++)
                {
                    var enemyTile = board.Tiles[random.Next(0, board.Tiles.Count)];

                    if(board.PieceAt(enemyTile) == null)
                    {
                        game.SpawnEnemy(enemyTile);

                    }
                }
            }

            if(GUILayout.Button("Remove Enemies"))
            {
                var enemies = FindObjectsOfType<EnemyView>();

                foreach(EnemyView e in enemies)
                {
                    var enemyObject = e.gameObject;
                    DestroyImmediate(enemyObject);
                }
            }
        }
    }
}

#endif
