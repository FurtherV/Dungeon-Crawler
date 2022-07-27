using System;
using System.Collections.Generic;
using System.IO;
using DungeonCrawler.Scripts.Utils;
using UnityEngine;

namespace DungeonCrawler.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MapEditor : BehaviourSingletonPersistent<MapEditor>
    {
        #region Serialized Fields

        [SerializeField] private string fileName = "testmap";

        #endregion

        private Mesh _mesh;

        private MeshFilter _meshFilter;

        #region Event Functions

        /// <inheritdoc />
        protected override void Awake()
        {
            if (!GameManager.Instance.editorEnabled)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
        }

        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            UpdateGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var grid = GameManager.Instance.Grid;
                var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                if (!grid.TryGetXYFromWorldPoint(worldPosition, out var x, out var y)) return;
                var cell = grid.GetCell(x, y);
                cell.IsWalkable = !cell.IsWalkable;
                grid.SetCell(x, y, cell);
                UpdateGrid();
            }

            if (Input.GetKeyDown(KeyCode.S)) Save();
        }

        #endregion

        private void UpdateGrid()
        {
            var grid = GameManager.Instance.Grid;
            MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out var vertices, out var uv, out var triangles);
            var vertexColors = new Color[vertices.Length];
            Array.Fill(vertexColors, Color.clear);

            var quadSize = new Vector3(1, 1) * grid.CellSize;
            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    var index = (x * grid.Height) + y;
                    var cell = grid.GetCell(x, y);
                    var pos = cell.GetWorldPoint() + (quadSize * 0.5f);
                    pos.z = -1;

                    var vertexColor = cell.IsWalkable ? Color.clear : new Color(0f, 0f, 0f, 0.75f);
                    Array.Fill(vertexColors, vertexColor, index * 4, 4);

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, pos, 0f, quadSize, Vector3.zero,
                        Vector3.one);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
            _mesh.SetColors(vertexColors);
        }

        private void Save()
        {
            var grid = GameManager.Instance.Grid;
            var unWalkableTiles = new List<Vector2Int>();
            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell.IsWalkable) continue;
                    unWalkableTiles.Add(new Vector2Int(x, y));
                }
            }

            var mapSettings = new MapSettings(unWalkableTiles.ToArray());

            var json = JsonUtility.ToJson(mapSettings, false);

            var folderPath = Application.dataPath;
            var filePath = Path.Combine(folderPath, "MapSettings", fileName + ".json");
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved map to {filePath}.");
        }
    }

    public class MapSettings
    {
        [SerializeField] private Vector2Int[] _unWalkableTiles;

        public MapSettings(Vector2Int[] unWalkableTiles)
        {
            _unWalkableTiles = unWalkableTiles;
        }
    }
}