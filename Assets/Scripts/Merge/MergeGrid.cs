using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
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

        [SerializeField] private LevelPriceData m_levelPriceData;
        
        private GridRenderer m_gridRenderer;
        private BulletFactory m_bulletFactory;
        private CurrencyManager m_currencyManager;

        private CameraController m_cameraController;
        private SimpleMover m_simpleMover;

        private Bullet[] m_bullets;
        private GridCell[] m_cells;
        private Vector3[] m_cellCenters;

        private List<Bullet> m_survivingBullets;
        
        private int[] m_gridLevelData;

        private float m_preLevelBulletSpeed;

        private const int gridWidth = 5;
        private const int gridHeight = 3;
        private const int m_gridSize = gridWidth * gridHeight;
        // const parameters might be dynamic in future
        
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
            m_currencyManager = DI.Resolve<CurrencyManager>();

            m_cameraController = DI.Resolve<CameraController>();
            m_simpleMover = DI.Resolve<SimpleMover>();
            
            m_bullets = new Bullet[m_gridSize];
            m_cells = new GridCell[m_gridSize];
            m_gridLevelData = new int[m_gridSize];

            GameEventSystem.AddListener<LevelLoadedEvent>(OnLevelLoaded);
            
            GameEventSystem.AddListener<BuyButtonPressedEvent>(OnBuyButtonPressed);
            GameEventSystem.AddListener<PlayButtonPressedEvent>(OnPlayButtonPressed);

            GameEventSystem.AddListener<BulletEnteredGunEvent>(OnBulletEnteredGun);
            
            if (!PlayerPrefs.HasKey(keyForFirstSave))
                CreateAndSaveFirstData();
        }

        private void Start()
        {
            m_cellCenters = m_gridRenderer.RenderGrid(m_center.position, gridWidth, gridHeight);
            m_gridLevelData = DataSystem.LoadArray<int>();
            
            var gameManager = DI.Resolve<GameManager>();
            m_preLevelBulletSpeed = gameManager.GameParameters.PlayerSpeed;
            
            CreateCells();
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
        
        private void OnLevelLoaded(object none)
        {
            LoadGrid();
        }
        
        private async void LoadGrid()
        {
            await Task.Delay(100); // wait for 100 milliseconds to ensure safe execution order.
            
            var bulletSize = m_gridRenderer.ItemSize;
            
            for (var i = 0; i < m_gridLevelData.Length; i++)
            {
                var level = m_gridLevelData[i];

                if (level == -1)
                    continue;
                
                var bullet = m_bulletFactory.Get(level, bulletSize);
                
                bullet.SetPosition(m_cellCenters[i]);
                bullet.ResetPreLevelHealth();

                m_bullets[i] = bullet;
                m_cells[i].SetIndex(i);
            }
        }

        private void OnPlayButtonPressed(object none)
        {
            m_survivingBullets = m_bullets.Where(bullet => bullet).ToList();
            
            var bulletsCenterPos = m_survivingBullets.GetCenterPosition();
            m_simpleMover.SetPosition(bulletsCenterPos);
            
            m_survivingBullets.SetGroupParent(m_simpleMover.transform);
            m_simpleMover.EnableMoving(true, m_preLevelBulletSpeed);
            
            m_cameraController.SetTarget(m_simpleMover.transform);
            m_cameraController.SetMode(FollowMode.PreLevel);
            
            m_cameraController.EnableFollowing(true);
        }
        
        private void OnBuyButtonPressed(object bulletLevel)
        {
            var price = m_levelPriceData.GetPriceOf((int)bulletLevel);

            if (m_currencyManager.TrySpend(price))
            {
                AddNewBullet(level: 1);
                SaveData();
            }
        }

        private void OnBulletEnteredGun(object enteredBullet)
        {
            var bullet = (Bullet)enteredBullet;

            if (m_survivingBullets.Contains(bullet))
            {
                m_survivingBullets.Remove(bullet);
                GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
            }

            if (m_survivingBullets.Count < 1)
                GameEventSystem.Invoke<GunsLoadedEvent>();
        }
        
        private void AddNewBullet(int level)
        {
            var emptyIndex = -1;
                
            for (var i = 0; i < m_bullets.Length; i++)
            {
                if (m_bullets[i])
                    continue;

                emptyIndex = i;
                break;
            }

            if (emptyIndex == -1)
                return;

            var size = m_gridRenderer.ItemSize;
            
            var bullet = m_bulletFactory.Get(level, size);
            
            bullet.SetPosition(m_cellCenters[emptyIndex]);
            bullet.ResetPreLevelHealth();
            
            UpdateCell(emptyIndex, bullet);
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
            var gameManager = DI.Resolve<GameManager>();
            var firstGameBulletCount = gameManager.GameParameters.InitialBulletCount;
            
            for (int index = 0; index < m_gridSize; index++)
            {
                m_gridLevelData[index] = index < firstGameBulletCount ? 1 : -1;
            }

            DataSystem.SaveArray(m_gridLevelData);
            PlayerPrefs.SetInt(keyForFirstSave, 1);
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelLoadedEvent>(OnLevelLoaded);
            
            GameEventSystem.RemoveListener<BuyButtonPressedEvent>(OnBuyButtonPressed);
            GameEventSystem.RemoveListener<PlayButtonPressedEvent>(OnPlayButtonPressed);
            
            GameEventSystem.RemoveListener<BulletEnteredGunEvent>(OnBulletEnteredGun);
        }
    }
}