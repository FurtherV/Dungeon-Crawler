using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Scripts.Utils;
using UnityEngine;

namespace DungeonCrawler.Scripts.GameCanvas
{
    public class GridPathfinder
    {
        private readonly Vector2Int[] _directions;
        private readonly Grid<GridCell> _grid;
        private readonly Dictionary<GridCell, GridCell[]> _neighboursCache;

        public GridPathfinder(Grid<GridCell> grid)
        {
            _grid = grid;
            _neighboursCache = new Dictionary<GridCell, GridCell[]>();
            _directions = new[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 1)
            };
            CacheNeighbours();
        }

        private void CacheNeighbours()
        {
            _neighboursCache.Clear();
            var neighbours = new List<GridCell>(8);
            for (var x = 0; x < _grid.Width; x++)
            {
                for (var y = 0; y < _grid.Height; y++)
                {
                    neighbours.Clear();
                    var current = _grid.GetCell(x, y);
                    for (var i = 0; i < _directions.Length; i++)
                    {
                        var direction = _directions[i];
                        var offsetX = direction.x;
                        var offsetY = direction.y;
                        if (_grid.HasCell(current.X + offsetX, current.Y + offsetY))
                            neighbours.Add(_grid.GetCell(current.X + offsetX, current.Y + offsetY));
                    }

                    _neighboursCache[current] = neighbours.ToArray();
                }
            }
        }

        public Path[] FindPathsInRadius(GridCell start, int radiusX, int radiusY)
        {
            var frontier = new PriorityQueue<GridCell, int>();
            frontier.Enqueue(0, start);
            var cameFrom = new Dictionary<GridCell, GridCell>();
            var costSoFar = new Dictionary<GridCell, int>();
            cameFrom[start] = null;
            costSoFar[start] = 0;

            while (!frontier.IsEmpty)
            {
                var current = frontier.Dequeue();
                if (Mathf.Abs(current.X - start.X) > radiusX) continue;
                if (Mathf.Abs(current.Y - start.Y) > radiusY) continue;

                foreach (var next in _neighboursCache[current])
                {
                    var newCost = costSoFar[current] + Cost(current, next);
                    if (costSoFar.ContainsKey(next) && (newCost >= costSoFar[next])) continue;
                    costSoFar[next] = newCost;
                    frontier.Enqueue(newCost, next);
                    cameFrom[next] = current;
                }
            }

            return cameFrom.Select(pair => ConstructPath(pair.Key, cameFrom)).Where(x => !x.IsEmpty).ToArray();
        }

        public Path FindPath(Vector3 start, Vector3 destination)
        {
            if (!_grid.TryGetXYFromWorldPoint(start, out var startX, out var startY)) return new Path();
            if (!_grid.TryGetXYFromWorldPoint(destination, out var destinationX, out var destinationY))
                return new Path();

            return FindPath(_grid.GetCell(startX, startY), _grid.GetCell(destinationX, destinationY));
        }

        public Path FindPath(GridCell start, GridCell destination)
        {
            if (start == destination) return new Path();

            var frontier = new PriorityQueue<GridCell, int>();
            frontier.Enqueue(0, start);
            var cameFrom = new Dictionary<GridCell, GridCell>();
            var costSoFar = new Dictionary<GridCell, int>();
            cameFrom[start] = null;
            costSoFar[start] = 0;

            while (!frontier.IsEmpty)
            {
                var current = frontier.Dequeue();

                if (current == destination) break;

                foreach (var next in _neighboursCache[current])
                {
                    var newCost = costSoFar[current] + Cost(current, next);
                    if (costSoFar.ContainsKey(next) && (newCost >= costSoFar[next])) continue;
                    costSoFar[next] = newCost;
                    var priority = newCost + Heuristic(next, destination);
                    frontier.Enqueue(priority, next);
                    cameFrom[next] = current;
                }
            }

            return frontier.IsEmpty ? new Path() : ConstructPath(destination, cameFrom);
        }

        private Path ConstructPath(GridCell destination, IReadOnlyDictionary<GridCell, GridCell> cameFrom)
        {
            if (!cameFrom.ContainsKey(destination)) return new Path();

            var path = new List<GridCell>();
            var current = destination;
            while ((current != null) && cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.RemoveAt(path.Count - 1);
            path.Reverse();
            return new Path(path.ToArray());
        }

        private int Cost(GridCell a, GridCell b)
        {
            if (a == b) return 0;

            // Artificially increase cost of diagonals because straighter paths are preferred.
            var isDiagonal = (Math.Abs(a.X - b.X) == 1) && (Math.Abs(a.Y - b.Y) == 1);
            return isDiagonal ? b.EnteringCost() + 1 : b.EnteringCost();
        }

        private int Heuristic(GridCell a, GridCell b)
        {
            return MovementCost.NORMAL * Mathf.Max(Mathf.Abs(a.X - b.X), Mathf.Abs(a.Y - b.Y));
        }
    }
}