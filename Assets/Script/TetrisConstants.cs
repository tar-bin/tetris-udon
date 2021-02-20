using UnityEngine;

namespace Script {
    public static class TetrisConstants {
        public static readonly Material[] Colors = {
            new Material(Shader.Find("Unlit/Color")) {color = Color.white},
            new Material(Shader.Find("Unlit/Color")) {color = Color.cyan},
            new Material(Shader.Find("Unlit/Color")) {color = Color.yellow},
            new Material(Shader.Find("Unlit/Color")) {color = Color.green},
            new Material(Shader.Find("Unlit/Color")) {color = Color.red},
            new Material(Shader.Find("Unlit/Color")) {color = Color.blue},
            new Material(Shader.Find("Unlit/Color")) {color = new Color(1.0f, 165.0f/255f, 0)},
            new Material(Shader.Find("Unlit/Color")) {color = Color.magenta},
        };

        public const int PositionMaxX = 10;
        public const int PositionMaxY = 20;
        public const int PositionSpawnX = 3;
        public const int PositionSpawnY = -3;
        public const int Angle0 = 0;
        public const int Angle90 = 1;
        public const int Angle180 = 2;
        public const int Angle270 = 3;
        public const int PieceTypeNone = 0;
        public const int PieceTypeI = 1;
        public const int PieceTypeO = 2;
        public const int PieceTypeS = 3;
        public const int PieceTypeZ = 4;
        public const int PieceTypeJ = 5;
        public const int PieceTypeL = 6;
        public const int PieceTypeT = 7;
    }
}