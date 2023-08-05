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
        
        private GridRenderer m_gridRenderer;

        private Bullet[] m_bullets;
        private Vector3[] m_cellCenters;
        
        private int[] m_gridLevelData;

        private BulletFactory m_bulletFactory;
        
        private const int gridWidth = 5;
        private const int gridHeight = 3;
        // const parameters might be dynamic in future

        private const int firstGameBulletCount = 12;
        private const string keyForFirstSave = "saved_once";


        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_gridRenderer = DI.Resolve<GridRenderer>();
            m_bulletFactory = DI.Resolve<BulletFactory>();

            m_bullets = new Bullet[gridWidth * gridHeight];
            m_gridLevelData = new int[gridWidth * gridHeight];
            
            if (!PlayerPrefs.HasKey(keyForFirstSave))
                CreateAndSaveFirstData();
        }

        private void Start()
        {
            m_cellCenters = m_gridRenderer.RenderGrid(m_center.position, gridWidth, gridHeight);
            m_gridLevelData = DataSystem.LoadArray<int>();
            
            LoadGrid();
        }

        private void LoadGrid()
        {
            for (var i = 0; i < m_gridLevelData.Length; i++)
            {
                var level = m_gridLevelData[i];

                if (level == -1)
                    continue;
                
                var bullet = m_bulletFactory.Get(level, size: 2);
                
                bullet.SetGridIndex(i);
                bullet.SetPosition(m_cellCenters[i]);
            }
        }

        public void UpdateCell(int index, Bullet bullet)
        {
            m_bullets[index] = bullet;
            m_gridLevelData[index] = bullet ? bullet.Level : -1;

            DataSystem.SaveArray(m_gridLevelData);
        }
        
        private void CreateAndSaveFirstData()
        {
            for (int index = 0; index < gridWidth * gridHeight; index++)
            {
                m_gridLevelData[index] = index < firstGameBulletCount ? 0 : -1;
            }

            DataSystem.SaveArray(m_gridLevelData);
            PlayerPrefs.SetInt(keyForFirstSave, 1);
        }
    }
}