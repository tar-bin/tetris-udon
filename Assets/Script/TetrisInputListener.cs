using UnityEngine;

namespace Script {
    public class TetrisInputListener : MonoBehaviour {
        private TetrisBlockSimulationModel _model;

        private void Start() {
            _model = gameObject.GetComponent<TetrisBlockSimulationModel>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                _model.MoveDown();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                _model.MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                _model.MoveRight();
            }
        }
    }
}