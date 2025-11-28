namespace PathfindingApp
{
    internal static class BFSPathFinderHelpers
    {

        private static List<(int, int)> ReconstructPath(
            Dictionary<(int, int), (int, int)> parent,
            (int, int) start, (int, int) end)
        {
            var path = new List<(int, int)>();
            var current = end;
            while (current != start)
            {
                path.Add(current);
                current = parent[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}