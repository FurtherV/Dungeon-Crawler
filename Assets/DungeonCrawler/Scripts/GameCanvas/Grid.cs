using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DungeonCrawler.Scripts.GameCanvas
{
    public class Grid<TCell>
    {
        private readonly TCell[,] _cells;
        private TextMesh[,] _debugText;

        public Grid(int width, int height, float cellSize, Vector3 origin,
            Func<Grid<TCell>, int, int, TCell> generatorFunc = null)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            Origin = origin;
            _cells = new TCell[Width, Height];

            if (generatorFunc == null) return;
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++) _cells[x, y] = generatorFunc.Invoke(this, x, y);
            }
        }

        public Vector3 Origin { get; }

        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public event EventHandler<GridChangedEventArgs> GridCellChanged;

        public bool TryGetXYFromWorldPoint(Vector3 position, out int x, out int y)
        {
            var gridRelativePos = position - Origin;
            x = Mathf.FloorToInt(gridRelativePos.x / CellSize);
            y = Mathf.FloorToInt(gridRelativePos.y / CellSize);
            return HasCell(x, y);
        }

        public Vector3 CellToWorldPoint(int x, int y)
        {
            return Origin + new Vector3(x * CellSize, y * CellSize);
        }

        public TCell GetCell(int x, int y)
        {
            if (!HasCell(x, y)) throw new IndexOutOfRangeException($"Grid has no cell at {x}-{y}.");
            return _cells[x, y];
        }

        public void SetCell(int x, int y, TCell value)
        {
            if (!HasCell(x, y)) throw new IndexOutOfRangeException($"Grid has no cell at {x}-{y}.");
            _cells[x, y] = value;
        }

        public bool HasCell(int x, int y)
        {
            return (x >= 0) && (x < Width) && (y >= 0) && (y < Height);
        }

        protected virtual void OnGridChanged(int x, int y)
        {
            GridCellChanged?.Invoke(this, new GridChangedEventArgs(x, y));
        }

        public void DebugDraw()
        {
            DebugDrawLines();
            DebugDrawText();
        }

        public void DebugDrawLines()
        {
            var color = Color.white;
            const float duration = 999f;

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Debug.DrawLine(CellToWorldPoint(x, y), CellToWorldPoint(x + 1, y), color, duration,
                        false);
                    Debug.DrawLine(CellToWorldPoint(x, y), CellToWorldPoint(x, y + 1), color, duration,
                        false);
                }
            }

            Debug.DrawLine(CellToWorldPoint(Width, Height), CellToWorldPoint(Width, 0),
                color, duration, false);
            Debug.DrawLine(CellToWorldPoint(Width, Height), CellToWorldPoint(0, Height),
                color, duration, false);
        }

        private void DebugDrawText()
        {
            if (_debugText != null)
                for (var x = 0; x < _debugText.GetLength(0); x++)
                {
                    for (var y = 0; y < _debugText.GetLength(1); y++)
                    {
                        Object.Destroy(_debugText[x, y].gameObject);
                        _debugText[x, y] = null;
                    }
                }
            else
                _debugText = new TextMesh[Width, Height];

            var rootObject = new GameObject("GridDebugHelper_Root");
            var rootTransform = rootObject.transform;
            rootTransform.position = Origin;

            _debugText = new TextMesh[Width, Height];

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var gameObject = new GameObject($"DebugText-{x}-{y}", typeof(TextMesh));
                    var transform = gameObject.transform;
                    transform.SetParent(rootTransform);
                    transform.localPosition = CellToWorldPoint(x, y) + (new Vector3(1, 1) * CellSize * 0.5f);
                    var textMesh = gameObject.GetComponent<TextMesh>();
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.text = GetCell(x, y).ToString();
                    textMesh.fontSize = Mathf.Max(1, Mathf.FloorToInt(CellSize * 3f));
                    textMesh.color = Color.white;
                    gameObject.GetComponent<MeshRenderer>().sortingOrder = 5000;

                    _debugText[x, y] = textMesh;
                }
            }

            GridCellChanged += (sender, gridArgs) =>
            {
                _debugText[gridArgs.X, gridArgs.Y].text = GetCell(gridArgs.X, gridArgs.Y).ToString();
            };
        }
    }

    public class GridChangedEventArgs : EventArgs
    {
        public GridChangedEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}