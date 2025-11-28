using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PathfindingApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TerrainGrid());
        }
    }

    public class TerrainGrid : Form
    {
        private TableLayoutPanel gridPanel;
        private Button buttonRun, buttonLoadMap;
        private ComboBox algoSelector;
        private Map currentMap;

        private readonly Dictionary<string, Func<IPathFinder>> algoFactory =
            new Dictionary<string, Func<IPathFinder>>(StringComparer.OrdinalIgnoreCase)
            {
                { "dfs", () => new DFSPathFinder() },
                { "bfs", () => new BFSPathFinder() },
                { "hill", () => new HillClimbingPathFinder() },
                { "best", () => new BestFirstPathFinder() },
                { "astar", () => new AStarPathFinder() },
                { "dijkstra", () => new DijkstraPathFinder() }
            };

        public TerrainGrid()
        {
            InitializeComponent();
            CreateDefaultMap();
            DrawGrid();
        }

        private void InitializeComponent()
        {
            gridPanel = new TableLayoutPanel();
            buttonRun = new Button();
            buttonLoadMap = new Button();
            algoSelector = new ComboBox();

            SuspendLayout();

            gridPanel.Location = new Point(20, 20);
            gridPanel.Size = new Size(360, 360);
            gridPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            algoSelector.Location = new Point(400, 40);
            algoSelector.Size = new Size(150, 30);
            algoSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            algoSelector.Items.AddRange(new string[] { "DFS", "BFS", "Hill", "Best", "AStar", "Dijkstra" });
            algoSelector.SelectedIndex = 0;

            buttonRun.Location = new Point(400, 80);
            buttonRun.Size = new Size(150, 30);
            buttonRun.Text = "Run Algorithm";
            buttonRun.Click += ButtonRun_Click;

            buttonLoadMap.Location = new Point(400, 120);
            buttonLoadMap.Size = new Size(150, 30);
            buttonLoadMap.Text = "Load Map";
            buttonLoadMap.Click += ButtonLoadMap_Click;

            ClientSize = new Size(600, 420);
            Controls.Add(gridPanel);
            Controls.Add(algoSelector);
            Controls.Add(buttonRun);
            Controls.Add(buttonLoadMap);
            Text = "Pathfinding Grid with Terrain";

            ResumeLayout(false);
        }

        private void ButtonRun_Click(object sender, EventArgs e)
        {
            string algoKey = algoSelector.SelectedItem.ToString();
            var pathFinder = algoFactory[algoKey.ToLower()]();
            List<(int r, int c)> path = (List<(int r, int c)>)pathFinder.FindPath(currentMap);

            if (path != null)
            {
                foreach (var cell in path)
                {
                    int r = cell.Item1;
                    int c = cell.Item2;
                    if ((r, c) != currentMap.Start && (r, c) != currentMap.End)
                        currentMap.Grid[r, c] = 4;
                }

                DrawGrid(); 
            }
            else
            {
                MessageBox.Show("No path found.");
            }
        }

        private void CreateDefaultMap()
        {
            int size = 12;
            int[,] grid = new int[size, size];
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    grid[r, c] = 1;

            currentMap = new Map(grid, (0, 0), (size - 1, size - 1));
        }

        private void ButtonLoadMap_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int[,] grid = LoadMapFromFile(ofd.FileName);
                        currentMap = new Map(grid, (0, 0), (grid.GetLength(0) - 1, grid.GetLength(1) - 1));
                        DrawGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to load map: " + ex.Message);
                    }
                }
            }
        }

        private int[,] LoadMapFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var parsed = lines
                .Select(line => line.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse).ToArray())
                .ToArray();

            int rows = parsed.Length;
            int cols = parsed[0].Length;
            int[,] grid = new int[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = parsed[r][c];

            return grid;
        }

        private void DrawGrid()
        {
            gridPanel.Controls.Clear();
            gridPanel.RowStyles.Clear();
            gridPanel.ColumnStyles.Clear();

            int rows = currentMap.Grid.GetLength(0);
            int cols = currentMap.Grid.GetLength(1);

            gridPanel.RowCount = rows;
            gridPanel.ColumnCount = cols;

            for (int r = 0; r < rows; r++)
            {
                gridPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

                for (int c = 0; c < cols; c++)
                {
                    if (r == 0)
                        gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));

                    var cell = new Panel
                    {
                        BackColor = GetCellColor(r, c),
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0),
                        Tag = (r, c)
                    };

                    cell.Click += Cell_Click;
                    gridPanel.Controls.Add(cell, c, r);
                }
            }
        }

        private Color GetCellColor(int r, int c)
        {
            int val = currentMap.Grid[r, c];

            return val switch
            {
                0 => Color.Black,        // blocked
                1 => Color.White,        // normal
                2 => Color.LightGreen,   // start
                3 => Color.Orange,       // goal
                4 => Color.LightBlue,    // path
                _ => Color.Gray
            };
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            Panel cell = (Panel)sender;
            var (r, c) = ((int, int))cell.Tag;

            // Toggle terrain type
            currentMap.Grid[r, c] = currentMap.Grid[r, c] == 0 ? 1 : 0;

            cell.BackColor = GetCellColor(r, c);
        }

     
        // Supporting Classes
     

        public class Map
        {
            public int[,] Grid { get; }
            public (int r, int c) Start { get; set; }
            public (int r, int c) End { get; set; }

            public Map(int[,] grid, (int, int) start, (int, int) end)
            {
                Grid = grid;
                Start = start;
                End = end;
            }
        }

        public interface IPathFinder
        {
            List<(int r, int c)> FindPath(Map currentMap);

        }

        internal class DFSPathFinder : IPathFinder
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

            private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> parent,
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


        public class BFSPathfinding:IPathFinder
            {
            public List<(int r, int c)> FindPath(Map currentMap)
            {
                throw new NotImplementedException();
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
            public class HillClimbingPathFinder : IPathFinder
            {
            public List<(int r, int c)> FindPath(Map currentMap)


            {
                throw new NotImplementedException();
                }
            }
            public class BestFirstPathFinder : IPathFinder
            {
            public List<(int r, int c)> FindPath(Map currentMap)


            {
                throw new NotImplementedException();
                }
            }
            public class AStarPathFinder : IPathFinder
            {
            public List<(int r, int c)> FindPath(Map currentMap)


            {
                throw new NotImplementedException();
                }
            }
        internal class DijkstraPathFinder : IPathFinder
        {
            private object a;

            public DijkstraPathFinder(object a)
            {
                this.a = a;
            }

            public DijkstraPathFinder()
            {
            }

            public override bool Equals(object? obj)
            {
                return obj is DijkstraPathFinder finder &&
                       EqualityComparer<object>.Default.Equals(a, finder.a);
            }

            public List<(int r, int c)> FindPath(TerrainGrid.Map map)
            {
                var rows = map.Grid.GetLength(0);
                var cols = map.Grid.GetLength(1);
                var visited = new bool[rows, cols];
                var dist = new Dictionary<(int, int), int>();
                var parent = new Dictionary<(int, int), (int, int)>();

                var pq = new SortedSet<(int r, int c)>(
                    Comparer<(int, int)>.Create((a, b) =>
                    {
                        int da = dist.ContainsKey(a) ? dist[a] : int.MaxValue;
                        int db = dist.ContainsKey(b) ? dist[b] : int.MaxValue;
                        return da == db ? a.GetHashCode().CompareTo(b.GetHashCode()) : da.CompareTo(db);
                    }));

                dist[map.Start] = 0;
                pq.Add(map.Start);

                int[][] directions = new int[][]
                {
            new[] {1, 0}, new[] {-1, 0}, new[] {0, 1}, new[] {0, -1}
                };

                while (pq.Count > 0)
                {
                    var current = pq.Min;
                    pq.Remove(current);

                    if (visited[current.r, current.c]) continue;
                    visited[current.r, current.c] = true;

                    if (current == map.End)
                        return ReconstructPath(parent, map.Start, map.End);

                    foreach (var d in directions)
                    {
                        int nr = current.r + d[0], nc = current.c + d[1];
                        if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                        if (map.Grid[nr, nc] == 0) continue;

                        var neighbor = (nr, nc);
                        int cost = 1; // uniform cost
                        int newDist = dist[current] + cost;

                        if (!dist.ContainsKey(neighbor) || newDist < dist[neighbor])
                        {
                            dist[neighbor] = newDist;
                            parent[neighbor] = current;
                            pq.Add(neighbor);
                        }
                    }
                }

                return null;
            }

            private List<(int r, int c)> ReconstructPath(Dictionary<(int, int), (int, int)> parent, (int r, int c) start, (int r, int c) end)
            {
                throw new NotImplementedException();
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a);
            }

            public override string? ToString()
            {
                return base.ToString();
            }
        }
    }
}
        
    

