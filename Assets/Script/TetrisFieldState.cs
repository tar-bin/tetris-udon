namespace Script {
    public class TetrisFieldState {
        public TetrisFieldState() {
            Data = new int[TetrisConstants.PositionMaxY, TetrisConstants.PositionMaxX];
        }

        public int[,] Data { get; }
    }
}