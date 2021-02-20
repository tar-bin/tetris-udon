using UnityEngine;

namespace Script {
    public class TetrisInputListener : MonoBehaviour {
        private TetrisBlockSimulationModel _model;

        private void Start() {
            _model = gameObject.GetComponent<TetrisBlockSimulationModel>();
        }

        private void Update() {
            // 左回転
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q)) {
                _model.TurnLeft();
            }
            // 右回転
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.E)) {
                _model.TurnRight();
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