using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Script.TetrisConstants;
using Random = UnityEngine.Random;

namespace Script {
    public class TetrisBlockSimulationModel : MonoBehaviour {
        public TetrisBoardViewModel boardViewModel;
        public TetrisNextPieceViewModel nextPieceViewModel;

        private TetrisFieldState _fieldState;
        private TetrisFieldState _nextFieldState;
        private TetrisPiece _currentPiece;
        private TetrisPiece _nextPiece;

        private int _frameCount;

        private void Start() {
            _fieldState = new TetrisFieldState();
            _nextFieldState = new TetrisFieldState();
            _currentPiece = TetrisPiece.Factory.CreateRandomPiece();
            _nextPiece = TetrisPiece.Factory.CreateRandomPiece();
            _frameCount = 0;

            // Spawn位置に補正
            _currentPiece.Pos.X = PositionSpawnX;
            _currentPiece.Pos.Y = PositionSpawnY;

            // ViewModel Bindings
            boardViewModel.State = _nextFieldState; //FIXME
            nextPieceViewModel.PieceData = _nextPiece;
        }

        private void Update() {
            //確定したら現在の状態に反映
            UpdatePiece();

            if (++_frameCount > 30) {
                _frameCount = 0;
                if (!MoveDown()) {
                    //現在の状態を固定
                    Array.Copy(_nextFieldState.Data, _fieldState.Data, _fieldState.Data.Length);

                    //削除処理
                    //TODO

                    //削除した行を詰める

                    //次のピースに入れ替え
                    _currentPiece = _nextPiece;
                    _nextPiece = TetrisPiece.Factory.CreateRandomPiece();

                    // Spawn位置に補正
                    _currentPiece.Pos.X = PositionSpawnX;
                    _currentPiece.Pos.Y = PositionSpawnY;
                }
            }
        }

        public bool MoveLeft() {
            var left = _currentPiece.Left;
            var posX = _currentPiece.Pos.X;
            var posY = _currentPiece.Pos.Y;

            // はみ出し判定
            if (posX + left <= 0) {
                return false;
            }

            // 左にブロックがあるか
            for (var i = 0; i < _currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < _currentPiece.Data.GetLength(1); j++) {
                    var block = _currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = _currentPiece.Pos.X + j - 1;
                    var y = _currentPiece.Pos.Y + i;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.Data[y, x] != 0) {
                        return false;
                    }
                }
            }

            _currentPiece.Pos.X -= 1;

            return true;
        }

        public bool MoveRight() {
            var right = _currentPiece.Right;

            // はみ出し判定
            if (_currentPiece.Pos.X + right >= PositionMaxX - 1) {
                return false;
            }

            // 右にブロックがあるか
            for (var i = 0; i < _currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < _currentPiece.Data.GetLength(1); j++) {
                    var block = _currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = _currentPiece.Pos.X + j + 1;
                    var y = _currentPiece.Pos.Y + i;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.Data[y, x] != 0) {
                        return false;
                    }
                }
            }

            _currentPiece.Pos.X += 1;

            return true;
        }

        public bool MoveDown() {
            var bottom = _currentPiece.Bottom;

            // はみ出し判定
            if (_currentPiece.Pos.Y + bottom >= PositionMaxY - 1) {
                return false;
            }

            // 下にブロックがあるか
            for (var i = 0; i < _currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < _currentPiece.Data.GetLength(1); j++) {
                    var block = _currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = _currentPiece.Pos.X + j;
                    var y = _currentPiece.Pos.Y + i + 1;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.Data[y, x] != 0) {
                        return false;
                    }
                }
            }

            _currentPiece.Pos.Y += 1;

            return true;
        }

        private void UpdatePiece() {
            //現在の状態をコピー
            Array.Copy(_fieldState.Data, _nextFieldState.Data, _fieldState.Data.Length);

            //ピースを合成
            for (var i = 0; i < _currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < _currentPiece.Data.GetLength(1); j++) {
                    var block = _currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = _currentPiece.Pos.X + j;
                    var y = _currentPiece.Pos.Y + i;
                    if (x >= 0 && x < PositionMaxX && y >= 0 && y < PositionMaxY) {
                        _nextFieldState.Data[y, x] = block;
                    }
                }
            }
        }
    }
}