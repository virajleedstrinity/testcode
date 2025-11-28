
using static PathfindingApp.TerrainGrid;

namespace PathfindingApp
{
    internal class BFSPathFinder : IPathFinder
    {
        public List<(int r, int c)> FindPath(TerrainGrid.Map map)
        {
            var rows = map.Grid.GetLength(0);
            var cols = map.Grid.GetLength(1);
            var visited = new bool[rows, cols];
            var parent = new Dictionary<(int, int), (int, int)>();

            var queue = new Queue<(int r, int c)>();
            queue.Enqueue(map.Start);
            visited[map.Start.r, map.Start.c] = true;

            int[][] directions =
            {
    new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 }
};

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == map.End)
                    return ReconstructPath(parent, map.Start, map.End);

                foreach (var d in directions)
                {
                    int nr = current.r + d[0];
                    int nc = current.c + d[1];

                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (visited[nr, nc] || map.Grid[nr, nc] == 0) continue;

                    visited[nr, nc] = true;
                    parent[(nr, nc)] = current;
                    queue.Enqueue((nr, nc));
                }
            }

            return null; // no path found
        }

        private List<(int, int)> ReconstructPath(
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

    internal class HillClimbingPathFinder : TerrainGrid.IPathFinder
        {
            public List<(int r, int c)> FindPath(TerrainGrid.Map currentMap)
            {
                throw new NotImplementedException();
            }
        }

        internal class DijkstraPathFinder : TerrainGrid.IPathFinder
        {
            public List<(int r, int c)> FindPath(TerrainGrid.Map currentMap)
            {
                throw new NotImplementedException();
            }
        }
    }
