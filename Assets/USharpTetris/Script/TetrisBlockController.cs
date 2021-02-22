using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TetrisBlockController : UdonSharpBehaviour {
    private const int PositionSpawnX = 3;
    private const int PositionSpawnY = -3;
    private const int PieceTypeNone = 0;
    private const int PieceTypeI = 1;
    private const int PieceTypeO = 2;
    private const int PieceTypeS = 3;
    private const int PieceTypeZ = 4;
    private const int PieceTypeJ = 5;
    private const int PieceTypeL = 6;
    private const int PieceTypeT = 7;
    private const int Angle0 = 0;
    private const int Angle90 = 1;
    private const int Angle180 = 2;
    private const int Angle270 = 3;

    public GameObject materials;
    public GameObject mainFieldRoot;
    public GameObject previewBlocks;

    private Material[] _colors;
    private int[][] _compositedField;
    private int[][][][] _piecePool;
    private int _autoMoveDownFrameCount;
    private int _deleteDelayFrameCount;
    private int _gameOverDelayFrameCount;
    private bool _isNeedPacking;
    private bool _isGameOver;
    private int[][] _currentField;
    private int[][][] _currentPiece;
    private int[][][] _nextPiece;

    private void Start() {
        _colors = new Material[9];
        _colors[0] = GetMaterial(0);
        _colors[1] = GetMaterial(1);
        _colors[2] = GetMaterial(2);
        _colors[3] = GetMaterial(3);
        _colors[4] = GetMaterial(4);
        _colors[5] = GetMaterial(5);
        _colors[6] = GetMaterial(6);
        _colors[7] = GetMaterial(7);
        _colors[8] = GetMaterial(8);

        _autoMoveDownFrameCount = 0;
        _deleteDelayFrameCount = 0;
        _gameOverDelayFrameCount = 0;

        _currentField = InitArrayMxN(20, 10);
        _currentPiece = CreateRandomPiece();
        _nextPiece = CreateRandomPiece();

        //ボードを再描画
        RedrawBoardBlocks();
        UpdatePreviewBlocks();
    }

    private void Update() {
        GetInputEvent();

        if (_isGameOver) {
            if (_gameOverDelayFrameCount > 60) {
                // 新規ゲーム設定
                UpdatePiece();
                Clear();
                _isGameOver = false;
                _gameOverDelayFrameCount = 0;
                //ボードを再描画
                RedrawBoardBlocks();
                return;
            }
            // 下からグレーアウト
            GameOver(_gameOverDelayFrameCount / 2);
            _gameOverDelayFrameCount++;
            //ボードを再描画
            RedrawBoardBlocks();
            return;
        }

        if (_isNeedPacking) {
            if (++_deleteDelayFrameCount > 10) {
                //削除した行を詰める
                PackLine();
                _isNeedPacking = false;
            }
        }

        // 一定フレームごとに下に移動
        if (++_autoMoveDownFrameCount > 24) {
            _autoMoveDownFrameCount = 0;
            if (!MoveDown()) {
                //次のピースに入れ替え
                UpdatePiece();

                //ゲームオーバーチェック
                if (_currentField[0][PositionSpawnX] != 0 ||
                    _currentField[0][PositionSpawnX + 1] != 0 ||
                    _currentField[0][PositionSpawnX + 2] != 0) {
                    _isGameOver = true;
                }
            }
        }

        //ボードを再描画
        RedrawBoardBlocks();
    }

    private void GetInputEvent() {
        // 左回転
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (RotateLeft()) {
                //回転が適用できた場合はカウントに猶予を追加
                FrameCountGrace(12);
            }
        }
        // 右回転
        if (Input.GetKeyDown(KeyCode.X)) {
            if (RotateRight()) {
                //回転が適用できた場合はカウントに猶予を追加
                FrameCountGrace(12);
            }
        }
        // ハードドロップ
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            while (MoveDown()) {
            }
            UpdatePiece();
        }
        // 左移動
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveLeft();
        }
        // 右移動
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveRight();
        }
        // 下移動
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveDown();
        }
    }

    private void RedrawBoardBlocks() {
        //現在のピースを表示用にフィールドに合成
        _compositedField = CompositePieceToField();

        for (var i = 0; i < 20; i++) {
            var rowObj = mainFieldRoot.transform.GetChild(i);

            for (var j = 0; j < 10; j++) {
                var columnObj = rowObj.GetChild(j);
                var meshRenderer = columnObj.GetComponent<MeshRenderer>();

                SetMaterial(meshRenderer, _compositedField[i][j]);
            }
        }
    }

    private void UpdatePreviewBlocks() {
        var preview = GetPiecePreview(_nextPiece);
        for (var i = 0; i < 4; i++) {
            var rowObj = previewBlocks.transform.GetChild(i);

            for (var j = 0; j < 4; j++) {
                var columnObj = rowObj.GetChild(j);
                var meshRenderer = columnObj.GetComponent<MeshRenderer>();

                SetMaterial(meshRenderer, preview[i][j]);
            }
        }
    }

    private void SetMaterial(Renderer render, int blockType) {
        render.sharedMaterial = _colors[blockType];
    }

    private Material GetMaterial(int index) {
        return materials.transform.GetChild(index).GetComponent<MeshRenderer>().sharedMaterial;
    }

    private int[][] CompositePieceToField() {
        //現在のフィールドの状態をコピー
        var compositedField = InitArrayMxN(20, 10);

        for (var i = 0; i < 20; i++) {
            for (var j = 0; j < 10; j++) {
                compositedField[i][j] = _currentField[i][j];
            }
        }

        //表示用フィールドにピースを合成
        var data = GetPieceData(_currentPiece);
        var size = GetPieceSize(data);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var block = data[i][j];
                if (block == 0) continue;

                var y = posY + i;
                var x = posX + j;
                if (x >= 0 && x < 10 && y >= 0 && y < 20) {
                    compositedField[y][x] = block;
                }
            }
        }

        return compositedField;
    }

    private void UpdateAndFixCurrentField() {
        //現在の状態を固定
        var compositedField = CompositePieceToField();
        for (var i = 0; i < 20; i++) {
            for (var j = 0; j < 10; j++) {
                _currentField[i][j] = compositedField[i][j];
            }
        }
    }

    private void Clear() {
        for (var i = 0; i < 20; i++) {
            for (var j = 0; j < 10; j++) {
                _currentField[i][j] = 0;
            }
        }
    }

    private void GameOver(int count) {
        var field = _currentField;

        var y = 20 - count - 1;
        if (y < 0) return;

        for (var j = 0; j < 10; j++) {
            if (field[y][j] != 0) {
                field[y][j] = 8;
            }
        }
    }

    private void UpdatePiece() {
        //ピースの位置を確定し、現在の状態を固定
        UpdateAndFixCurrentField();

        //削除処理
        if (DeleteFilledLine()) {
            _isNeedPacking = true;
            _autoMoveDownFrameCount = 0;
        }

        _currentPiece = _nextPiece;
        _nextPiece = CreateRandomPiece();

        UpdatePreviewBlocks();
    }

    private bool DeleteFilledLine() {
        var field = _currentField;
        var isNeedPacking = false;
        for (var i = 0; i < 20; i++) {
            //ラインが埋まっているかチェック
            var isFilled = true;
            for (var j = 0; j < 10; j++) {
                if (field[i][j] != 0) {
                    continue;
                }
                isFilled = false;
                break;
            }

            //ラインが埋まっている場合ラインをクリア
            if (isFilled) {
                for (var j = 0; j < 10; j++) {
                    field[i][j] = 0;
                }
                isNeedPacking = true;
            }
        }
        return isNeedPacking;
    }

    private void PackLine() {
        var packedField = InitArrayMxN(20, 10);
        var y = 20 - 1;
        for (var i = 20 - 1; i >= 0; i--) {
            //ラインにブロックが存在するかチェック
            var isExistBlock = false;
            for (var j = 0; j < 10; j++) {
                if (_currentField[i][j] != 0) {
                    isExistBlock = true;
                    break;
                }
            }

            //ブロックが存在する場合ラインを詰めてコピー
            if (isExistBlock) {
                for (var j = 0; j < 10; j++) {
                    packedField[y][j] = _currentField[i][j];
                }
                y--;
            }
        }
        // パッキングを適用
        for (var i = 0; i < 20; i++) {
            for (var j = 0; j < 10; j++) {
                _currentField[i][j] = packedField[i][j];
            }
        }
    }

    private void FrameCountGrace(int count) {
        _autoMoveDownFrameCount -= count;
    }

    private bool RotateLeft() {
        var data = GetPieceData(_currentPiece);
        var type = GetPieceType(_currentPiece);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        //タイプOは無条件でtrue
        if (type == PieceTypeO) {
            return true;
        }

        var rotatedData = GetRotateLeftData(data);
        var currentAngle = GetPieceAngle(_currentPiece);

        //https://tetris.wiki/Super_Rotation_System
        var wallKickData = InitArrayMxN(5, 2);
        int nextAngle;
        if (type == PieceTypeI) {
            switch (currentAngle) {
                case Angle0: //0->L
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {+2, 0}, {-1, +2}, {+2, -1}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 2;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = -1;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = 2;
                    wallKickData[4][1] = -1;
                    nextAngle = Angle270;
                    break;
                case Angle90: //R->0
                    // wallKickData = new[,] {{0, 0}, {+2, 0}, {-1, 0}, {+2, +1}, {-1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 2;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = 2;
                    wallKickData[3][1] = 1;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle0;
                    break;
                case Angle180: //2->R
                    // wallKickData = new[,] {{0, 0}, {+1, 0}, {-2, 0}, {+1, -2}, {-2, +1}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -2;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = 1;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = -2;
                    wallKickData[4][1] = 1;
                    nextAngle = Angle90;
                    break;
                default: //L->2
                    // wallKickData = new[,] {{0, 0}, {-2, 0}, {+1, 0}, {-2, -1}, {+1, +2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -2;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = -2;
                    wallKickData[3][1] = -1;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle180;
                    break;
            }
        } else {
            switch (currentAngle) {
                case Angle0: //0->L
                    // wallKickData = new[,] {{0, 0}, {1, 0}, {1, 1}, {0, -2}, {1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = 1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle270;
                    break;
                case Angle90: //R->0
                    // wallKickData = new[,] {{0, 0}, {1, 0}, {1, -1}, {0, 2}, {1, 2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = -1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle0;
                    break;
                case Angle180: //2->R
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {-1, 1}, {0, -2}, {-1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = 1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle90;
                    break;
                default: //L->2
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {-1, -1}, {0, 2}, {-1, 2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = -1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle180;
                    break;
            }
        }

        for (var i = 0; i < 5; i++) {
            var offsetX = wallKickData[i][0];
            var offsetY = -wallKickData[i][1];
            if (CheckAndApplyRotate(rotatedData, offsetX, offsetY)) {
                SetPiecePosX(_currentPiece, posX + offsetX);
                SetPiecePosY(_currentPiece, posY + offsetY);
                SetPieceAngle(_currentPiece, nextAngle);
                return true;
            }
        }

        return false;
    }

    private bool RotateRight() {
        var data = GetPieceData(_currentPiece);
        var type = GetPieceType(_currentPiece);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        //タイプOは無条件でtrue
        if (type == PieceTypeO) {
            return true;
        }

        var rotatedData = GetRotateRightData(data);
        var currentAngle = GetPieceAngle(_currentPiece);

        //https://tetris.wiki/Super_Rotation_System
        var wallKickData = InitArrayMxN(5, 2);
        int nextAngle;
        if (type == PieceTypeI) {
            switch (currentAngle) {
                case Angle0: //0->R
                    // wallKickData = new[,] {{0, 0}, {-2, 0}, {+1, 0}, {-2, -1}, {+1, +2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -2;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = -2;
                    wallKickData[3][1] = -1;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle90;
                    break;
                case Angle90: //R->2
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {+2, 0}, {-1, +2}, {+2, -1}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 2;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = -1;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = 2;
                    wallKickData[4][1] = -1;
                    nextAngle = Angle180;
                    break;
                case Angle180: //2->L
                    // wallKickData = new[,] {{0, 0}, {+2, 0}, {-1, 0}, {+2, +1}, {-1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 2;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = 2;
                    wallKickData[3][1] = 1;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle270;
                    break;
                default: //L->0
                    // wallKickData = new[,] {{0, 0}, {+1, 0}, {-2, 0}, {+1, -2}, {-2, +1}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -2;
                    wallKickData[2][1] = 0;
                    wallKickData[3][0] = 1;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = -2;
                    wallKickData[4][1] = 1;
                    nextAngle = Angle0;
                    break;
            }
        } else {
            switch (currentAngle) {
                case Angle0: //0->R
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {-1, 1}, {0, -2}, {-1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = 1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle90;
                    break;
                case Angle90: //R->2
                    // wallKickData = new[,] {{0, 0}, {1, 0}, {1, -1}, {0, 2}, {1, 2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = -1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle180;
                    break;
                case Angle180: //2->L
                    // wallKickData = new[,] {{0, 0}, {1, 0}, {1, 1}, {0, -2}, {1, -2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = 1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = 1;
                    wallKickData[2][1] = 1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = -2;
                    wallKickData[4][0] = 1;
                    wallKickData[4][1] = -2;
                    nextAngle = Angle270;
                    break;
                default: //L->0
                    // wallKickData = new[,] {{0, 0}, {-1, 0}, {-1, -1}, {0, 2}, {-1, 2}};
                    wallKickData[0][0] = 0;
                    wallKickData[0][1] = 0;
                    wallKickData[1][0] = -1;
                    wallKickData[1][1] = 0;
                    wallKickData[2][0] = -1;
                    wallKickData[2][1] = -1;
                    wallKickData[3][0] = 0;
                    wallKickData[3][1] = 2;
                    wallKickData[4][0] = -1;
                    wallKickData[4][1] = 2;
                    nextAngle = Angle0;
                    break;
            }
        }

        for (var i = 0; i < 5; i++) {
            var offsetX = wallKickData[i][0];
            var offsetY = -wallKickData[i][1];
            if (CheckAndApplyRotate(rotatedData, offsetX, offsetY)) {
                SetPiecePosX(_currentPiece, posX + offsetX);
                SetPiecePosY(_currentPiece, posY + offsetY);
                SetPieceAngle(_currentPiece, nextAngle);
                return true;
            }
        }

        return false;
    }


    private bool CheckAndApplyRotate(int[][] data, int offsetX, int offsetY) {
        var size = GetPieceSize(data);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        // 回転後にブロックと衝突するか
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var block = data[i][j];
                if (block == 0) continue;

                var x = posX + j + offsetX;
                var y = posY + i + offsetY;
                if (x < 0 || x >= 10 || y < 0 || y >= 20 ||
                    _currentField[y][x] != 0) {
                    // 衝突する場合は回転を適用しない
                    return false;
                }
            }
        }

        // 回転を適用
        SetPieceData(_currentPiece, data);

        return true;
    }

    private void MoveLeft() {
        var data = GetPieceData(_currentPiece);
        var left = GetLeft(data);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        // はみ出し判定
        if (posX + left <= 0) {
            return;
        }

        // 左にブロックがあるか
        var size = GetPieceSize(data);
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var block = data[i][j];
                if (block == 0) continue;

                var x = posX + j - 1;
                var y = posY + i;
                if (x >= 0 && x < 10 && y >= 0 && y < 20 &&
                    _currentField[y][x] != 0) {
                    return;
                }
            }
        }

        SetPiecePosX(_currentPiece, posX - 1);
    }

    private void MoveRight() {
        var data = GetPieceData(_currentPiece);
        var right = GetRight(data);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        // はみ出し判定
        if (posX + right >= 10 - 1) {
            return;
        }

        // 右にブロックがあるか
        var size = GetPieceSize(data);
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var block = data[i][j];
                if (block == 0) continue;

                var x = posX + j + 1;
                var y = posY + i;
                if (x >= 0 && x < 10 && y >= 0 && y < 20 &&
                    _currentField[y][x] != 0) {
                    return;
                }
            }
        }

        SetPiecePosX(_currentPiece, posX + 1);
    }

    private bool MoveDown() {
        var data = GetPieceData(_currentPiece);
        var bottom = GetBottom(data);
        var posX = GetPiecePosX(_currentPiece);
        var posY = GetPiecePosY(_currentPiece);

        // はみ出し判定
        if (posY + bottom >= 20 - 1) {
            return false;
        }

        // 下にブロックがあるか
        var size = GetPieceSize(data);
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var block = data[i][j];
                if (block == 0) continue;

                var x = posX + j;
                var y = posY + i + 1;
                if (x >= 0 && x < 10 && y >= 0 && y < 20 &&
                    _currentField[y][x] != 0) {
                    return false;
                }
            }
        }

        SetPiecePosY(_currentPiece, posY + 1);

        return true;
    }

    private int GetBottom(int[][] data) {
        var size = GetPieceSize(data);
        for (var i = size - 1; i >= 0; i--) {
            for (var j = size - 1; j >= 0; j--) {
                if (data[i][j] != 0) {
                    return i;
                }
            }
        }
        return -100; //Invalid
    }

    private int GetLeft(int[][] data) {
        var size = GetPieceSize(data);
        for (var j = 0; j < size; j++) {
            for (var i = 0; i < size; i++) {
                if (data[i][j] != 0) {
                    return j;
                }
            }
        }
        return -100; //Invalid
    }

    private int GetRight(int[][] data) {
        var size = GetPieceSize(data);
        for (var j = size - 1; j >= 0; j--) {
            for (var i = size - 1; i >= 0; i--) {
                if (data[i][j] != 0) {
                    return j;
                }
            }
        }
        return -100; //Invalid
    }

    private int[][] GetRotateRightData(int[][] data) {
        var size = GetPieceSize(data);
        var result = InitArrayMxN(size, size);

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                result[j][size - i - 1] = data[i][j];
            }
        }

        return result;
    }

    private int[][] GetRotateLeftData(int[][] data) {
        var size = GetPieceSize(data);
        var result = InitArrayMxN(size, size);

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                result[size - j - 1][i] = data[i][j];
            }
        }

        return result;
    }

    private void Reset() {
        _piecePool = new int[7][][][];
        //I
        // {0, 0, 0, 0}, // □ □ □ □
        // {1, 1, 1, 1}, // ■ ■ ■ ■
        // {0, 0, 0, 0}, // □ □ □ □
        // {0, 0, 0, 0}, // □ □ □ □ 
        var dataI = InitArrayMxN(4, 4);
        dataI[1][0] = 1;
        dataI[1][1] = 1;
        dataI[1][2] = 1;
        dataI[1][3] = 1;
        var previewI = InitArrayMxN(4, 4);
        previewI[1][0] = 1;
        previewI[1][1] = 1;
        previewI[1][2] = 1;
        previewI[1][3] = 1;
        _piecePool[0] = CreatePiece(PieceTypeI, dataI, previewI);
        //O
        // {0, 0, 0, 0}, // □ □ □ □
        // {0, 2, 2, 0}, // □ ■ ■ □
        // {0, 2, 2, 0}, // □ ■ ■ □
        // {0, 0, 0, 0}, // □ □ □ □
        var dataO = InitArrayMxN(4, 4);
        dataO[1][1] = 2;
        dataO[1][2] = 2;
        dataO[2][1] = 2;
        dataO[2][2] = 2;
        var previewO = InitArrayMxN(4, 4);
        previewO[1][1] = 2;
        previewO[1][2] = 2;
        previewO[2][1] = 2;
        previewO[2][2] = 2;
        _piecePool[1] = CreatePiece(PieceTypeO, dataO, previewO);
        //S
        // {0, 3, 3}, // □ ■ ■
        // {3, 3, 0}, // ■ ■ □
        // {0, 0, 0}, // □ □ □
        var dataS = InitArrayMxN(3, 3);
        dataS[0][1] = 3;
        dataS[0][2] = 3;
        dataS[1][0] = 3;
        dataS[1][1] = 3;
        var previewS = InitArrayMxN(4, 4);
        previewS[1][1] = 3;
        previewS[1][2] = 3;
        previewS[2][0] = 3;
        previewS[2][1] = 3;
        _piecePool[2] = CreatePiece(PieceTypeS, dataS, previewS);
        //Z
        // {4, 4, 0}, // ■ ■ □
        // {0, 4, 4}, // □ ■ ■
        // {0, 0, 0}, // □ □ □
        var dataZ = InitArrayMxN(3, 3);
        dataZ[0][0] = 4;
        dataZ[0][1] = 4;
        dataZ[1][1] = 4;
        dataZ[1][2] = 4;
        var previewZ = InitArrayMxN(4, 4);
        previewZ[1][0] = 4;
        previewZ[1][1] = 4;
        previewZ[2][1] = 4;
        previewZ[2][2] = 4;
        _piecePool[3] = CreatePiece(PieceTypeZ, dataZ, previewZ);
        //J
        // {0, 0, 5}, // □ □ ■
        // {5, 5, 5}, // ■ ■ ■
        // {0, 0, 0}, // □ □ □
        var dataJ = InitArrayMxN(3, 3);
        dataJ[0][2] = 5;
        dataJ[1][0] = 5;
        dataJ[1][1] = 5;
        dataJ[1][2] = 5;
        var previewJ = InitArrayMxN(4, 4);
        previewJ[1][2] = 5;
        previewJ[2][0] = 5;
        previewJ[2][1] = 5;
        previewJ[2][2] = 5;
        _piecePool[4] = CreatePiece(PieceTypeJ, dataJ, previewJ);
        //L
        // {6, 0, 0}, // ■ □ □
        // {6, 6, 6}, // ■ ■ ■
        // {0, 0, 0}, // □ □ □
        var dataL = InitArrayMxN(3, 3);
        dataL[0][0] = 6;
        dataL[1][0] = 6;
        dataL[1][1] = 6;
        dataL[1][2] = 6;
        var previewL = InitArrayMxN(4, 4);
        previewL[1][0] = 6;
        previewL[2][0] = 6;
        previewL[2][1] = 6;
        previewL[2][2] = 6;
        _piecePool[5] = CreatePiece(PieceTypeL, dataL, previewL);
        //T
        // {0, 7, 0}, // □ ■ □
        // {7, 7, 7}, // ■ ■ ■
        // {0, 0, 0}, // □ □ □
        var dataT = InitArrayMxN(3, 3);
        dataT[0][1] = 7;
        dataT[1][0] = 7;
        dataT[1][1] = 7;
        dataT[1][2] = 7;
        var previewT = InitArrayMxN(4, 4);
        previewT[1][1] = 7;
        previewT[2][0] = 7;
        previewT[2][1] = 7;
        previewT[2][2] = 7;
        _piecePool[6] = CreatePiece(PieceTypeT, dataT, previewT);
    }

    private int[][][] getOnePiece() {
        var piecePoolNextLength = _piecePool.Length - 1;
        var nextPiecePool = new int[piecePoolNextLength][][][];
        var nextIndex = UnityEngine.Random.Range(0, _piecePool.Length - 1);
        var nextPiece = _piecePool[nextIndex];
        var j = 0;
        for (var i = 0; i < _piecePool.Length; i++) {
            if (nextIndex != i) {
                nextPiecePool[j] = _piecePool[i];
                j++;
            }
        }
        _piecePool = nextPiecePool;
        return nextPiece;
    }

    private int[][][] CreateRandomPiece() {
        //完全ランダムではなく全種類が同じ頻度で取得されるようにする
        if (_piecePool == null || _piecePool.Length == 0) {
            Reset();
        }
        return getOnePiece();
    }

    private int[][] InitArrayMxN(int m, int n) {
        var data = new int[m][];
        for (var i = 0; i < m; i++) {
            data[i] = new int[n];
        }
        return data;
    }

    private int[][][] CreatePiece(int type, int[][] data, int[][] preview) {
        var piece = new int[3][][];
        piece[0] = new int[3][];
        piece[0][0] = new int[4];
        piece[0][0][0] = type;
        piece[0][0][1] = Angle0;
        piece[0][0][2] = PositionSpawnX;
        piece[0][0][3] = PositionSpawnY;
        piece[1] = data;
        piece[2] = preview;
        return piece;
    }

    private int[][] GetPieceData(int[][][] piece) {
        return piece[1];
    }

    private void SetPieceData(int[][][] piece, int[][] data) {
        piece[1] = data;
    }

    private int[][] GetPiecePreview(int[][][] piece) {
        return piece[2];
    }

    private int GetPieceType(int[][][] piece) {
        return piece[0][0][0];
    }

    private int GetPieceAngle(int[][][] piece) {
        return piece[0][0][1];
    }

    private void SetPieceAngle(int[][][] piece, int angle) {
        piece[0][0][1] = angle;
    }

    private int GetPiecePosX(int[][][] piece) {
        return piece[0][0][2];
    }

    private void SetPiecePosX(int[][][] piece, int pos) {
        piece[0][0][2] = pos;
    }

    private int GetPiecePosY(int[][][] piece) {
        return piece[0][0][3];
    }

    private void SetPiecePosY(int[][][] piece, int pos) {
        piece[0][0][3] = pos;
    }

    private int GetPieceSize(int[][] data) {
        return data.Length;
    }
}