namespace Creating_UI
{
    internal class Map
    {
        internal object Grid;
        internal int Cols;
        internal float Rows;

        public Map(int[,] grid, (int, int) start, (int, int) goal)
        {
        }

        public object Start { get; internal set; }
    }
}