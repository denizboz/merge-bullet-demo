using UnityEngine;
using Utility;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameParametersSO m_gameParameters;
        [SerializeField] private GameObject m_playerPrefab;

        private LevelSO[] m_levels;
        private GameObject m_levelObjects;

        private int m_currentLevelNumber;
        
        private const string levelsDirectory = "Levels";
        private const string keyForLevelNumber = "level_number";
        
        public bool MainStageOver { get; private set; }
        public bool FinalStageOver { get; private set; }
        public GameParametersSO GameParameters => m_gameParameters;

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyForLevelNumber))
                PlayerPrefs.SetInt(keyForLevelNumber, 0);

            m_levels = Resources.LoadAll<LevelSO>(levelsDirectory);
            m_currentLevelNumber = PlayerPrefs.GetInt(keyForLevelNumber);
            
            LoadLevel();
        }

        private void LoadLevel()
        {
            var currentLevel = m_levels[m_currentLevelNumber];
        }

        private void OnFinishLineReached(object none)
        {
            MainStageOver = true;
        }
        
        private void OnLevelSuccess(object none)
        {
            FinalStageOver = true;
            m_currentLevelNumber++;

            PlayerPrefs.SetInt(keyForLevelNumber, m_currentLevelNumber);
        }

        private void OnLevelFail(object none)
        {
            FinalStageOver = true;
        }
    }
}