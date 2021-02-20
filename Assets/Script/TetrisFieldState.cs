using System;

namespace Script {
    public class TetrisFieldState {
        public TetrisFieldState() {
            CurrentField = new int[TetrisConstants.PositionMaxY, TetrisConstants.PositionMaxX];
        }

        public int[,] CurrentField { get; }
        public TetrisPiece CurrentPiece { get; set; }
        
        public int[,] CompositePieceToField() {
            var compositedField = new int[TetrisConstants.PositionMaxY, TetrisConstants.PositionMaxX];
            Array.Copy(CurrentField, compositedField, CurrentField.Length);
            for (var i = 0; i < CurrentPiece.Data.GetLength(0); i++) {
                for (var j = 0; j < CurrentPiece.Data.GetLength(1); j++) {
                    var block = CurrentPiece.Data[i, j];
                    if (block == 0) continue;

                    var x = CurrentPiece.Pos.X + j;
                    var y = CurrentPiece.Pos.Y + i;
                    if (x >= 0 && x < TetrisConstants.PositionMaxX && y >= 0 && y < TetrisConstants.PositionMaxY) {
                        compositedField[y, x] = block;
                    }
                }
            }
            return compositedField;
        }

        public void UpdateAndFixCurrentField() {
            var compositedField = CompositePieceToField();
            //現在の状態を固定
            Array.Copy(compositedField, CurrentField, CurrentField.Length);
        }
    }
}