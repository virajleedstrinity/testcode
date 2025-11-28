namespace PathfindingApp
{
    public class Map
    {
        internal object Grid;
        private int[,] grid;
        private (int, int) value1;
        private (int, int) value2;

        public Map(int[,] grid, (int, int) value1, (int, int) value2)
        {
            this.grid = grid;
            this.value1 = value1;
            this.value2 = value2;
        }

        public object Start { get; internal set; }
        public object End { get; internal set; }
    }
}