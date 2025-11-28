
using static PathfindingApp.TerrainGrid;

namespace PathfindingApp
{
    internal class AStarPathFinder : IPathFinder
    {
        public object OpenListSorts { get; internal set; }

        public List<(int, int)> FindPath(Map map)
        {
            throw new NotImplementedException();
        }

        public List<(int r, int c)> FindPath(TerrainGrid.Map map)
        {
            throw new NotImplementedException();
        }
    }
}