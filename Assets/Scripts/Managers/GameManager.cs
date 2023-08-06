using System.Linq;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Managers
{
    public class GameManager : MonoBehaviour, IDependency
    {
        [SerializeField] private GameParametersSO m_gameParameters;

        private LevelSO[] m_levels;
        private GameObject m_levelObjects;

        private int m_actualLevelNumber;
        private int m_appLevelNumber;
        
        private const string levelsDirectory = "Levels";
        private const string keyForLevelNumber = "level_number";
        
        public GameParametersSO GameParameters => m_gameParameters;


        public void Bind()
        {
            DI.Bind(this);
        }

        private void Update()
        {
            // TODO: DEBUG PURPOSE --
            if (Input.GetKeyDown(KeyCode.Return))
                GameEventSystem.Invoke<LevelWonEvent>();
            else if (Input.GetKeyDown(KeyCode.Space))
                GameEventSystem.Invoke<LevelFailedEvent>();
            // TODO: DEBUG PURPOSE -- 
        }

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyForLevelNumber))
                PlayerPrefs.SetInt(keyForLevelNumber, 0);

            m_levels = Resources.LoadAll<LevelSO>(levelsDirectory).OrderBy(level => level.Number).ToArray();
            
            m_appLevelNumber = PlayerPrefs.GetInt(keyForLevelNumber);
            m_actualLevelNumber = m_appLevelNumber % m_levels.Length;
            
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelSuccess);
            GameEventSystem.AddListener<NextButtonPressedEvent>(OnNextButtonPressed);
        }

        private void Start()
        {
            LoadLevel();
        }

        private void LoadLevel()
        {
            var currentLevel = m_levels[m_actualLevelNumber];
            
            if (m_levelObjects)
                Destroy(m_levelObjects);

            m_levelObjects = Instantiate(currentLevel.LevelObjects);
            
            GameEventSystem.Invoke<LevelLoadedEvent>(m_appLevelNumber);
        }

        private void OnLevelSuccess(object none)
        {
            m_appLevelNumber++;
            m_actualLevelNumber = m_appLevelNumber % m_levels.Length;
            
            PlayerPrefs.SetInt(keyForLevelNumber, m_appLevelNumber);
        }

        private void OnNextButtonPressed(object none) => LoadLevel();
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelSuccess);
            GameEventSystem.RemoveListener<NextButtonPressedEvent>(OnNextButtonPressed);
        }
    }
}