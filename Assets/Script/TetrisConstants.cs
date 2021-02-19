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
            new Material(Shader.Find("Unlit/Color")) {color = new Color(255, 69, 0)},
            new Material(Shader.Find("Unlit/Color")) {color = Color.magenta},
        };

        public const int PositionMaxX = 10;
        public const int PositionMaxY = 20;
        public const int PositionSpawnX = 3;
        public const int PositionSpawnY = -3;
    }
}