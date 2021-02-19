using UnityEngine;

namespace Script {
    public class TetrisNextPieceViewModel : MonoBehaviour {
        public TetrisPiece PieceData;

        // log
        private ILogger _logger;

        private void Awake() {
            _logger = new Logger(Debug.unityLogger);
        }

        // Update is called once per frame
        private void Update() {
            UpdateBlocks();
        }

        private void UpdateBlocks() {
            var y = PieceData.Data.GetLength(0);
            var x = PieceData.Data.GetLength(1);
            if (y > 20 || x > 10) {
                _logger.Log($"over block size {x} x {y}");
                return;
            }

            for (var i = 0; i < y; i++) {
                var rowObj = gameObject.transform.GetChild(i);

                for (var j = 0; j < x; j++) {
                    var columnObj = rowObj.GetChild(j);
                    var meshRenderer = columnObj.GetComponent<MeshRenderer>();

                    SetMaterial(meshRenderer, PieceData.Data[i, j]);
                }
            }
        }

        private static void SetMaterial(Renderer render, int blockType) {
            render.sharedMaterial = TetrisConstants.Colors[blockType];
        }
    }
}