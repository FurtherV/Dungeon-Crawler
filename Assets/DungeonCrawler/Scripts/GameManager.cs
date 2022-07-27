using DungeonCrawler.Scripts.GameCanvas;
using DungeonCrawler.Scripts.Utils;
using UnityEngine;

namespace DungeonCrawler.Scripts
{
    public class GameManager : BehaviourSingletonPersistent<GameManager>
    {
        #region Serialized Fields

        public int width = 10;
        public int height = 10;
        public float cellSize = 10f;
        public bool editorEnabled;

        #endregion

        public Grid<GridCell> Grid { get; private set; }


        public GridPathfinder Pathfinder { get; private set; }

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();
            Grid = new Grid<GridCell>(width, height, cellSize, transform.position,
                (grid, x, y) => new GridCell(grid, x, y));
            Pathfinder = new GridPathfinder(Grid);
        }

        private void Start()
        {
            CenterCamera();
        }

        #endregion

        private void CenterCamera()
        {
            var mainCamera = Camera.main!;
            var cameraPos =
                transform.position + (new Vector3(width * cellSize, height * cellSize) * 0.5f);
            cameraPos.z = -10;
            mainCamera.transform.position = cameraPos;
            mainCamera.orthographicSize = height * cellSize * 0.55f;
        }
    }
}