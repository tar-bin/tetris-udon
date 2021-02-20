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
        private TetrisPiece _nextPiece;

        private int _frameCount;

        private void Start() {
            _nextPiece = TetrisPiece.Factory.CreateRandomPiece();
            _fieldState = new TetrisFieldState {
                CurrentPiece = TetrisPiece.Factory.CreateRandomPiece()
            };
            _frameCount = 0;

            // Spawn位置に補正
            _fieldState.CurrentPiece.Pos.X = PositionSpawnX;
            _fieldState.CurrentPiece.Pos.Y = PositionSpawnY;

            // ViewModel Bindings
            boardViewModel.FieldState = _fieldState; //FIXME
            nextPieceViewModel.PieceData = _nextPiece;
        }

        private void Update() {
            // 30フレームごとに下に移動
            if (++_frameCount > 30) {
                _frameCount = 0;
                if (!MoveDown()) {
                    //ピースの位置を確定し、現在の状態を固定
                    _fieldState.UpdateAndFixCurrentField();

                    //削除処理
                    //TODO

                    //削除した行を詰める

                    //次のピースに入れ替え
                    _fieldState.CurrentPiece = _nextPiece;
                    _nextPiece = TetrisPiece.Factory.CreateRandomPiece();
                    nextPieceViewModel.PieceData = _nextPiece;

                    // Spawn位置に補正
                    _fieldState.CurrentPiece.Pos.X = PositionSpawnX;
                    _fieldState.CurrentPiece.Pos.Y = PositionSpawnY;
                }
            }
        }

        public bool MoveLeft() {
            var currentPiece = _fieldState.CurrentPiece;
            var left = currentPiece.Left;
            var posX = currentPiece.Pos.X;
            var posY = currentPiece.Pos.Y;

            // はみ出し判定
            if (posX + left <= 0) {
                return false;
            }

            // 左にブロックがあるか
            for (var i = 0; i < currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < currentPiece.Data.GetLength(1); j++) {
                    var block = currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = currentPiece.Pos.X + j - 1;
                    var y = currentPiece.Pos.Y + i;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.CurrentField[y, x] != 0) {
                        return false;
                    }
                }
            }

            currentPiece.Pos.X -= 1;

            return true;
        }

        public bool MoveRight() {
            var currentPiece = _fieldState.CurrentPiece;
            var right = currentPiece.Right;

            // はみ出し判定
            if (currentPiece.Pos.X + right >= PositionMaxX - 1) {
                return false;
            }

            // 右にブロックがあるか
            for (var i = 0; i < currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < currentPiece.Data.GetLength(1); j++) {
                    var block = currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = currentPiece.Pos.X + j + 1;
                    var y = currentPiece.Pos.Y + i;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.CurrentField[y, x] != 0) {
                        return false;
                    }
                }
            }

            currentPiece.Pos.X += 1;

            return true;
        }

        public bool MoveDown() {
            var currentPiece = _fieldState.CurrentPiece;
            var bottom = currentPiece.Bottom;

            // はみ出し判定
            if (currentPiece.Pos.Y + bottom >= PositionMaxY - 1) {
                return false;
            }

            // 下にブロックがあるか
            for (var i = 0; i < currentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < currentPiece.Data.GetLength(1); j++) {
                    var block = currentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = currentPiece.Pos.X + j;
                    var y = currentPiece.Pos.Y + i + 1;
                    if (x >= 0 && x < PositionMaxX &&
                        y >= 0 && y < PositionMaxY &&
                        _fieldState.CurrentField[y, x] != 0) {
                        return false;
                    }
                }
            }

            currentPiece.Pos.Y += 1;

            return true;
        }
    }
}