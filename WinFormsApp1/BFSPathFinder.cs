using static PathfindingApp.TerrainGrid;

namespace PathfindingApp
{
    internal class DFSPathFinder :  IPathFinder
    {
        public List<(int r, int c)> FindPath(TerrainGrid.Map map)
        {
            var rows = map.Grid.GetLength(0);
            var cols = map.Grid.GetLength(1);
            var visited = new bool[rows, cols];
            var parent = new Dictionary<(int, int), (int, int)>();

            bool found = DFS(map.Start, map.End, map, visited, parent);
            return found ? ReconstructPath(parent, map.Start, map.End) : null;
        }

        private List<(int r, int c)> ReconstructPath(Dictionary<(int, int), (int, int)> parent, (int r, int c) start, (int r, int c) end)
        {
            throw new NotImplementedException();
        }

        private bool DFS((int r, int c) current, (int r, int c) goal,
                         TerrainGrid.Map map, bool[,] visited,
                         Dictionary<(int, int), (int, int)> parent)
        {
            if (current == goal) return true;

            visited[current.r, current.c] = true;

            int[][] directions = { new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };

            foreach (var d in directions)
            {
                int nr = current.r + d[0], nc = current.c + d[1];
                if (nr < 0 || nr >= map.Grid.GetLength(0) || nc < 0 || nc >= map.Grid.GetLength(1))
                    continue;
                if (visited[nr, nc] || map.Grid[nr, nc] == 0) continue;

                parent[(nr, nc)] = current;
                if (DFS((nr, nc), goal, map, visited, parent)) return true;
            }

            return false;
        }

        internal class HillClimbingPathFinder : IPathFinder
        {
            public List<(int r, int c)> FindPath(TerrainGrid.Map currentMap)
            {
                throw new NotImplementedException();
            }
        }

        internal class DijkstraPathFinder : IPathFinder
        {
            public List<(int r, int c)> FindPath(TerrainGrid.Map currentMap)
            {
                throw new NotImplementedException();
            }
        }
    }
}
