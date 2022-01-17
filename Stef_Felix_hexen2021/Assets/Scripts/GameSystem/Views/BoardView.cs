using System;
using BoardSystem;
using UnityEngine;

namespace GameSystem.Views
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] public TileViewFactory TileViewFactory;
        private Board<HexPieceView> _model;

        public int EnemyAmount = 0;
        public void Start()
        {
            GameLoop.Instance.Initialized += OnGameLoopInitialized;
        }

        private void OnGameLoopInitialized(object sender, EventArgs e)
        {
            _model = GameLoop.Instance.Board;
        }
    }

}