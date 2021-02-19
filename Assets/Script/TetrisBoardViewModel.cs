using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Script {
    public class TetrisBoardViewModel : MonoBehaviour {
        public TetrisFieldState State;

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
            var y = State.Data.GetLength(0);
            var x = State.Data.GetLength(1);
            if (y > 20 || x > 10) {
                _logger.Log($"over block size {x} x {y}");
                return;
            }

            for (var i = 0; i < y; i++) {
                var rowObj = gameObject.transform.GetChild(i);

                for (var j = 0; j < x; j++) {
                    var columnObj = rowObj.GetChild(j);
                    var meshRenderer = columnObj.GetComponent<MeshRenderer>();

                    SetMaterial(meshRenderer, State.Data[i, j]);
                }
            }
        }

        private static void SetMaterial(Renderer render, int blockType) {
            render.sharedMaterial = TetrisConstants.Colors[blockType];
        }
    }
}