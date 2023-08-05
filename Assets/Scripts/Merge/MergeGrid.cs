using System;
using CommonTools.Runtime.DependencyInjection;
using Managers;
using PlayerSpace;
using UnityEngine;
using Utility;

namespace Merge
{
    public class MergeGrid : MonoBehaviour, IDependency
    {
        [SerializeField] private Transform m_center;
        [SerializeField] private GridCell m_cellPrefab;
        
        private GridRenderer m_gridRenderer;
        private BulletFactory m_bulletFactory;

        private Bullet[] m_bullets;
        private GridCell[] m_cells;
        private Vector3[] m_cellCenters;
        
        private int[] m_gridLevelData;

        private const int gridWidth = 5;
        private const int gridHeight = 3;
        private const int m_gridSize = gridWidth * gridHeight;
        // const parameters might be dynamic in future
        
        private const int firstGameBulletCount = 12;
        private const string keyForFirstSave = "saved_once";

        public Bullet BulletAtIndex(int index) => m_bullets[index];
        public int IndexOfBullet(Bullet bullet) => Array.IndexOf(m_bullets, bullet);
        

        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_gridRenderer = DI.Resolve<GridRenderer>();
            m_bulletFactory = DI.Resolve<BulletFactory>();

            m_bullets = new Bullet[m_gridSize];
            m_cells = new GridCell[m_gridSize];
            m_gridLevelData = new int[m_gridSize];

            if (!PlayerPrefs.HasKey(keyForFirstSave))
                CreateAndSaveFirstData();
        }

        private void Start()
        {
            m_cellCenters = m_gridRenderer.RenderGrid(m_center.position, gridWidth, gridHeight);
            m_gridLevelData = DataSystem.LoadArray<int>();
            
            CreateCells();
            LoadGrid();
        }

        private void CreateCells()
        {
            var cellSize = m_gridRenderer.CellSize;

            for (var i = 0; i < m_cellCenters.Length; i++)
            {
                var cell = Instantiate(m_cellPrefab, transform);
                
                cell.SetIndex(i);
                cell.transform.position = m_cellCenters[i];
                cell.transform.localScale = new Vector3(cellSize, 1f, cellSize);
                
                m_cells[i] = cell;
            }
        }
        
        private void LoadGrid()
        {
            for (var i = 0; i < m_gridLevelData.Length; i++)
            {
                var level = m_gridLevelData[i];

                if (level == -1)
                    continue;
                
                var bullet = m_bulletFactory.Get(level, size: 2);
                bullet.SetPosition(m_cellCenters[i]);

                m_bullets[i] = bullet;
                m_cells[i].SetIndex(i);
            }
        }

        public void UpdateCell(int index, Bullet bullet)
        {
            m_bullets[index] = bullet;
            m_gridLevelData[index] = bullet ? bullet.Level : -1;
        }

        public void SaveData()
        {
            DataSystem.SaveArray(m_gridLevelData);
        }
        
        private void CreateAndSaveFirstData()
        {
            for (int index = 0; index < m_gridSize; index++)
            {
                m_gridLevelData[index] = index < firstGameBulletCount ? 0 : -1;
            }

            DataSystem.SaveArray(m_gridLevelData);
            PlayerPrefs.SetInt(keyForFirstSave, 1);
        }
    }
}