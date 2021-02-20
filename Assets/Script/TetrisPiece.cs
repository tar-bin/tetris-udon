using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Script {
    public class TetrisPiece {
        public class Position {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public int Type { get; private set; }

        public Position Pos { get; } = new Position {
            X = TetrisConstants.PositionSpawnX,
            Y = TetrisConstants.PositionSpawnY,
        };

        public int[,] Preview { get; private set; }
        public int[,] Data { get; private set; }
        public int Angle { get; set; } = TetrisConstants.Angle0;

        public int GetTop() {
            for (var i = 0; i < Data.GetLength(0); i++) {
                for (var j = 0; j < Data.GetLength(1); j++) {
                    if (Data[i, j] != 0) {
                        return i;
                    }
                }
            }
            throw new InvalidProgramException();
        }

        public int GetBottom() {
            for (var i = Data.GetLength(0) - 1; i >= 0; i--) {
                for (var j = Data.GetLength(1) - 1; j >= 0; j--) {
                    if (Data[i, j] != 0) {
                        return i;
                    }
                }
            }
            throw new InvalidProgramException();
        }

        public int GetLeft() {
            for (var j = 0; j < Data.GetLength(1); j++) {
                for (var i = 0; i < Data.GetLength(0); i++) {
                    if (Data[i, j] != 0) {
                        return j;
                    }
                }
            }
            throw new InvalidProgramException();
        }

        public int GetRight() {
            for (var j = Data.GetLength(1) - 1; j >= 0; j--) {
                for (var i = Data.GetLength(0) - 1; i >= 0; i--) {
                    if (Data[i, j] != 0) {
                        return j;
                    }
                }
            }
            throw new InvalidProgramException();
        }

        public int[,] GetRotateRightData() {
            var rows = Data.GetLength(0);
            var columns = Data.GetLength(1);
            var result = new int[columns, rows];

            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < columns; j++) {
                    result[j, rows - i - 1] = Data[i, j];
                }
            }

            return result;
        }

        public int[,] GetRotateLeftData() {
            var rows = Data.GetLength(0);
            var columns = Data.GetLength(1);
            var result = new int[columns, rows];

            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < columns; j++) {
                    result[columns - j - 1, i] = Data[i, j];
                }
            }

            return result;
        }

        public static class Factory {
            private static readonly List<TetrisPiece> _piecePool = new List<TetrisPiece>();

            private static void Reset() {
                //I
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeI,
                    Data = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {1, 1, 1, 1}, // ■ ■ ■ ■
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 0, 0, 0}, // □ □ □ □ 
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {1, 1, 1, 1}, // ■ ■ ■ ■
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 0, 0, 0}, // □ □ □ □ 
                    }
                });
                //O
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeO,
                    Data = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 2, 2, 0}, // □ ■ ■ □
                        {0, 2, 2, 0}, // □ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 2, 2, 0}, // □ ■ ■ □
                        {0, 2, 2, 0}, // □ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
                //S
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeS,
                    Data = new[,] {
                        {0, 3, 3}, // □ ■ ■
                        {3, 3, 0}, // ■ ■ □
                        {0, 0, 0}, // □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 3, 3, 0}, // □ ■ ■ □
                        {3, 3, 0, 0}, // ■ ■ □ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
                //Z
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeZ,
                    Data = new[,] {
                        {4, 4, 0}, // ■ ■ □
                        {0, 4, 4}, // □ ■ ■
                        {0, 0, 0}, // □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {4, 4, 0, 0}, // ■ ■ □ □
                        {0, 4, 4, 0}, // □ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
                //J
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeJ,
                    Data = new[,] {
                        {0, 0, 5}, // □ □ ■
                        {5, 5, 5}, // ■ ■ ■
                        {0, 0, 0}, // □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 0, 5, 0}, // □ □ ■ □
                        {5, 5, 5, 0}, // ■ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
                //L
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeL,
                    Data = new[,] {
                        {6, 0, 0}, // ■ □ □
                        {6, 6, 6}, // ■ ■ ■
                        {0, 0, 0}, // □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {6, 0, 0, 0}, // ■ □ □ □
                        {6, 6, 6, 0}, // ■ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
                //T
                _piecePool.Add(new TetrisPiece {
                    Type = TetrisConstants.PieceTypeT,
                    Data = new[,] {
                        {0, 7, 0}, // □ ■ □
                        {7, 7, 7}, // ■ ■ ■
                        {0, 0, 0}, // □ □ □
                    },
                    Preview = new[,] {
                        {0, 0, 0, 0}, // □ □ □ □
                        {0, 7, 0, 0}, // □ ■ □ □
                        {7, 7, 7, 0}, // ■ ■ ■ □
                        {0, 0, 0, 0}, // □ □ □ □
                    },
                });
            }

            public static TetrisPiece CreateRandomPiece() {
                //完全ランダムではなく全種類が同じ頻度で取得されるようにする
                if (_piecePool.Count == 0) {
                    Reset();
                }
                var c = Random.Range(0, _piecePool.Count - 1);
                var tetrisPiece = _piecePool[c];
                _piecePool.Remove(tetrisPiece);
                return tetrisPiece;
            }
        }
    }
}