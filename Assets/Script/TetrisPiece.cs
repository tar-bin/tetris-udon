using System;
using Random = UnityEngine.Random;

namespace Script {
    public class TetrisPiece {
        public class Position {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public Position Pos { get; } = new Position();
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
            public static TetrisPiece CreateRandomPiece() {
                var c = Random.Range(1, 8);
                switch (c) {
                    case 1:
                        //I
                        return new TetrisPiece {
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
                        };
                    case 2:
                        //O
                        return new TetrisPiece {
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
                        };
                    case 3:
                        //S
                        return new TetrisPiece {
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
                        };
                    case 4:
                        //Z
                        return new TetrisPiece {
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
                        };
                    case 5:
                        //J
                        return new TetrisPiece {
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
                        };
                    case 6:
                        //L
                        return new TetrisPiece {
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
                        };
                    default:
                        //T
                        return new TetrisPiece {
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
                        };
                }
            }
        }
    }
}