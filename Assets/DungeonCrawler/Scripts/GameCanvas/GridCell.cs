using System;
using UnityEngine;

namespace DungeonCrawler.Scripts.GameCanvas
{
    public class GridCell
    {
        public GridCell(Grid<GridCell> grid, int x, int y)
        {
            Grid = grid;
            X = x;
            Y = y;
        }


        public Grid<GridCell> Grid { get; }
        public int X { get; }
        public int Y { get; }

        public bool IsDifficultTerrain { get; set; } = false;

        public bool IsWalkable { get; set; } = true;

        public Vector3 GetWorldPoint()
        {
            return Grid.CellToWorldPoint(X, Y);
        }

        public int EnteringCost(bool ignoreDifficultTerrain = false)
        {
            if (ignoreDifficultTerrain) return MovementCost.NORMAL;

            return IsDifficultTerrain ? MovementCost.DIFFICULT : MovementCost.NORMAL;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsWalkable ? "1" : "0";
        }
    }
}