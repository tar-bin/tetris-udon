using System;
using UnityEngine;
using static Script.TetrisConstants;

namespace Script {
    public class TetrisBlockSimulationModel : MonoBehaviour {
        public TetrisBoardViewModel boardViewModel;
        public TetrisNextPieceViewModel nextPieceViewModel;

        private TetrisFieldState _fieldState;
        private TetrisPiece _nextPiece;

        private int _autoMoveDownFrameCount;
        private int _deleteDelayFrameCount;
        private bool _isNeedPacking;

        private void Start() {
            _nextPiece = TetrisPiece.Factory.CreateRandomPiece();
            _fieldState = new TetrisFieldState {
                CurrentPiece = TetrisPiece.Factory.CreateRandomPiece()
            };
            _autoMoveDownFrameCount = 0;

            // Spawn位置に補正
            _fieldState.CurrentPiece.Pos.X = PositionSpawnX;
            _fieldState.CurrentPiece.Pos.Y = PositionSpawnY;

            // ViewModel Bindings
            boardViewModel.FieldState = _fieldState; //FIXME
            nextPieceViewModel.PieceData = _nextPiece;
        }

        private void Update() {
            if (_isNeedPacking) {
                if (++_autoMoveDownFrameCount > 10) {
                    //削除した行を詰める
                    PackLine();
                    _isNeedPacking = false;
                }
            }

            // 一定フレームごとに下に移動
            if (++_autoMoveDownFrameCount > 24) {
                _autoMoveDownFrameCount = 0;
                if (!MoveDown()) {
                    //ピースの位置を確定し、現在の状態を固定
                    _fieldState.UpdateAndFixCurrentField();

                    //削除処理
                    if (DeleteFilledLine()) {
                        _isNeedPacking = true;
                        _autoMoveDownFrameCount = 0;
                    }

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

        private bool DeleteFilledLine() {
            var field = _fieldState.CurrentField;
            var isNeedPacking = false;
            for (var i = 0; i < field.GetLength(0); i++) {
                //ラインが埋まっているかチェック
                var isFilled = true;
                for (var j = 0; j < field.GetLength(1); j++) {
                    if (field[i, j] != 0) {
                        continue;
                    }
                    isFilled = false;
                    break;
                }

                //ラインが埋まっている場合ラインをクリア
                if (isFilled) {
                    for (var j = 0; j < field.GetLength(1); j++) {
                        field[i, j] = 0;
                    }
                    isNeedPacking = true;
                }
            }
            return isNeedPacking;
        }

        private void PackLine() {
            var field = _fieldState.CurrentField;
            var packedField = new int[PositionMaxY, PositionMaxX];
            var y = field.GetLength(0) - 1;
            for (var i = field.GetLength(0) - 1; i >= 0; i--) {
                //ラインにブロックが存在するかチェック
                var isExistBlock = false;
                for (var j = 0; j < field.GetLength(1); j++) {
                    if (field[i, j] != 0) {
                        isExistBlock = true;
                        break;
                    }
                }

                //ブロックが存在する場合ラインを詰めてコピー
                if (isExistBlock) {
                    for (var j = 0; j < field.GetLength(1); j++) {
                        packedField[y, j] = field[i, j];
                    }
                    y--;
                }
            }
            // パッキングを適用
            Array.Copy(packedField, _fieldState.CurrentField, _fieldState.CurrentField.Length);
        }

        public void FrameCountGrace(int count) {
            _autoMoveDownFrameCount -= count;
        }

        public bool RotateLeft() {
            var currentPiece = _fieldState.CurrentPiece;
            var rotatedData = currentPiece.GetRotateLeftData();
            var currentAngle = currentPiece.Angle;

            int[] xs;
            int[] ys;
            int nextAngle;
            switch (currentAngle) {
                case Angle0:
                    xs = new[] {0, 1, 1, 0, 1,};
                    ys = new[] {0, 0, 1, -2, -2,};
                    nextAngle = Angle270;
                    break;
                case Angle90:
                    xs = new[] {0, 1, 1, 0, 1,};
                    ys = new[] {0, 0, -1, 2, 2,};
                    nextAngle = Angle0;
                    break;
                case Angle180:
                    xs = new[] {0, -1, -1, 0, -1,};
                    ys = new[] {0, 0, 1, -2, -2,};
                    nextAngle = Angle90;
                    break;
                default:
                    xs = new[] {0, -1, -1, 0, -1,};
                    ys = new[] {0, -1, -1, 2, 2,};
                    nextAngle = Angle180;
                    break;
            }

            for (var i = 0; i < 5; i++) {
                var offsetX = xs[i];
                var offsetY = ys[i];
                if (CheckAndApplyRotate(rotatedData, currentPiece, offsetX, offsetY)) {
                    currentPiece.Pos.X += offsetX;
                    currentPiece.Pos.Y += offsetY;
                    currentPiece.Angle = nextAngle;
                    return true;
                }
            }
            
            return false;
        }

        public bool RotateRight() {
            var currentPiece = _fieldState.CurrentPiece;
            var rotatedData = currentPiece.GetRotateRightData();
            var currentAngle = currentPiece.Angle;

            int[] xs;
            int[] ys;
            int nextAngle;
            switch (currentAngle) {
                case Angle0:
                    xs = new[] {0, -1, -1, 0, -1,};
                    ys = new[] {0, 0, 1, -2, -2,};
                    nextAngle = Angle90;
                    break;
                case Angle90:
                    xs = new[] {0, 1, 1, 0, 1,};
                    ys = new[] {0, 0, -1, 2, 2,};
                    nextAngle = Angle180;
                    break;
                case Angle180:
                    xs = new[] {0, 1, 1, 0, 1,};
                    ys = new[] {0, 0, 1, -2, -2,};
                    nextAngle = Angle270;
                    break;
                default:
                    xs = new[] {0, -2, -2, 0, -1,};
                    ys = new[] {0, 0, -1, 2, 2,};
                    nextAngle = Angle0;
                    break;
            }

            for (var i = 0; i < 5; i++) {
                var offsetX = xs[i];
                var offsetY = ys[i];
                if (CheckAndApplyRotate(rotatedData, currentPiece, offsetX, offsetY)) {
                    currentPiece.Pos.X += offsetX;
                    currentPiece.Pos.Y += offsetY;
                    currentPiece.Angle = nextAngle;
                    return true;
                }
            }
            
            return false;
        }

        private bool CheckAndApplyRotate(int[,] rotatedData, TetrisPiece currentPiece, int offsetX, int offsetY) {
            // 回転後にブロックと衝突するか
            for (var i = 0; i < rotatedData.GetLength(0); i++) {
                for (var j = 0; j < rotatedData.GetLength(1); j++) {
                    var block = rotatedData[i, j];
                    if (block == 0) continue;

                    var x = currentPiece.Pos.X + j + offsetX;
                    var y = currentPiece.Pos.Y + i + offsetY;
                    if (x < 0 || x >= PositionMaxX ||
                        y < 0 || y >= PositionMaxY ||
                        _fieldState.CurrentField[y, x] != 0) {
                        // 衝突する場合は回転を適用しない
                        return false;
                    }
                }
            }

            // 回転を適用
            Array.Copy(rotatedData, currentPiece.Data, currentPiece.Data.Length);

            return true;
        }

        public bool MoveLeft() {
            var currentPiece = _fieldState.CurrentPiece;
            var left = currentPiece.GetLeft();
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
            var right = currentPiece.GetRight();

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
            var bottom = currentPiece.GetBottom();

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