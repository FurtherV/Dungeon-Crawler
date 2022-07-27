using System;
using System.Linq;
using UnityEngine;

namespace DungeonCrawler.Scripts.GameCanvas
{
    public class Path
    {
        private readonly GridCell[] _pathCells;
        private Vector3[] _worldPath;

        /// <summary>
        ///     Creates an empty path.
        /// </summary>
        public Path()
        {
            _pathCells = Array.Empty<GridCell>();
            IsEmpty = true;
        }

        /// <summary>
        ///     Creates a path with given grid cells.
        /// </summary>
        /// <param name="pathCells"></param>
        public Path(GridCell[] pathCells)
        {
            _pathCells = pathCells;
            IsEmpty = _pathCells.Length == 0;
        }

        public bool IsEmpty { get; }

        public int GetCost()
        {
            return IsEmpty ? 0 : _pathCells.Sum(input => input.EnteringCost());
        }

        public GridCell[] GetCellPath()
        {
            return _pathCells;
        }

        public Vector3[] GetWorldPath()
        {
            return _worldPath ??= Array.ConvertAll(_pathCells, input => input.GetWorldPoint());
        }

        public void DebugDraw(Color? color = null)
        {
            if (_pathCells.Length == 0) return;
            var cellSize = _pathCells[0].Grid.CellSize;

            var worldPath = GetWorldPath();
            for (var index = 0; index < worldPath.Length - 1; index++)
            {
                var a = worldPath[index];
                var b = worldPath[index + 1];
                var offset = new Vector3(cellSize / 2, cellSize / 2);
                Debug.DrawLine(a + offset, b + offset, color.GetValueOrDefault(Color.green), 15f);
            }
        }
    }
}