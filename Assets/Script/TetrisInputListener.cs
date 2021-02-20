using UnityEngine;

namespace Script {
    public class TetrisInputListener : MonoBehaviour {
        private TetrisBlockSimulationModel _model;

        private void Start() {
            _model = gameObject.GetComponent<TetrisBlockSimulationModel>();
        }

        private void Update() {
            // 左回転
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E)) {
                if (_model.RotateLeft()) {
                    //回転が適用できた場合はカウントに猶予を追加
                    _model.FrameCountDecrease(12);
                }
            }
            // 右回転
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.R)) {
                if (_model.RotateRight()) {
                    //回転が適用できた場合はカウントに猶予を追加
                    _model.FrameCountDecrease(12);
                }
            }
            // ハードドロップ
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                while (_model.MoveDown()) {}
            }
            // 左移動
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                _model.MoveLeft();
            }
            // 右移動
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                _model.MoveRight();
            }
            // 下移動
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                _model.MoveDown();
            }
        }
    }
}