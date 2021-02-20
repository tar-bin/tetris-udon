using UnityEngine;

namespace Script {
    public class TetrisPiece {
        private TetrisPiece() {
            Pos = new Position();
        }

        public class Position {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public Position Pos { get; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int[,] Data { get; private set; }

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
                            Top = 1,
                            Bottom = 1,
                            Left = 0,
                            Right = 3,
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
                            Top = 1,
                            Bottom = 2,
                            Left = 1,
                            Right = 2,
                        };
                    case 3:
                        //S
                        return new TetrisPiece {
                            Data = new[,] {
                                {0, 0, 0, 0}, // □ □ □ □
                                {0, 3, 3, 0}, // □ ■ ■ □
                                {3, 3, 0, 0}, // ■ ■ □ □
                                {0, 0, 0, 0}, // □ □ □ □
                            },
                            Top = 1,
                            Bottom = 2,
                            Left = 0,
                            Right = 2,
                        };
                    case 4:
                        //Z
                        return new TetrisPiece {
                            Data = new[,] {
                                {0, 0, 0, 0}, // □ □ □ □
                                {4, 4, 0, 0}, // ■ ■ □ □
                                {0, 4, 4, 0}, // □ ■ ■ □
                                {0, 0, 0, 0}, // □ □ □ □
                            },
                            Top = 1,
                            Bottom = 2,
                            Left = 0,
                            Right = 2,
                        };
                    case 5:
                        //J
                        return new TetrisPiece {
                            Data = new[,] {
                                {0, 0, 0, 0}, // □ □ □ □
                                {0, 0, 5, 0}, // □ □ ■ □
                                {5, 5, 5, 0}, // ■ ■ ■ □
                                {0, 0, 0, 0}, // □ □ □ □
                            },
                            Top = 1,
                            Bottom = 2,
                            Left = 0,
                            Right = 2,
                        };
                    case 6:
                        //L
                        return new TetrisPiece {
                            Data = new[,] {
                                {0, 0, 0, 0}, // □ □ □ □
                                {6, 0, 0, 0}, // ■ □ □ □
                                {6, 6, 6, 0}, // ■ ■ ■ □
                                {0, 0, 0, 0}, // □ □ □ □
                            },
                            Top = 1,
                            Bottom = 2,
                            Left = 0,
                            Right = 2,
                        };
                    default:
                        //T
                        return new TetrisPiece {
                            Data = new[,] {
                                {0, 0, 0, 0}, // □ □ □ □
                                {0, 7, 0, 0}, // □ ■ □ □
                                {7, 7, 7, 0}, // ■ ■ ■ □
                                {0, 0, 0, 0}, // □ □ □ □
                            },
                            Top = 1,
                            Bottom = 2,
                            Left = 0,
                            Right = 2,
                        };
                }
            }
        }
    }
}