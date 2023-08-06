using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Managers
{
    public class CurrencyManager : MonoBehaviour, IDependency
    {
        private int m_currentAmount;

        private const string keyForCurrency = "currency";
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyForCurrency))
                SetFirstGameAmount();

            GameEventSystem.AddListener<CurrencyEarnedEvent>(OnCurrencyEarned);
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelSuccess);
        }

        private void Start()
        {
            m_currentAmount = PlayerPrefs.GetInt(keyForCurrency);
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
        }

        private void OnCurrencyEarned(object change)
        {
            m_currentAmount += (int)change;
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
            
            // data is saved at successful level end
        }

        public bool TrySpend(int amount)
        {
            if (m_currentAmount < amount)
                return false;

            m_currentAmount -= amount;
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
            
            PlayerPrefs.SetInt(keyForCurrency, m_currentAmount);
            
            return true;
        }

        private void OnLevelSuccess(object none)
        {
            PlayerPrefs.SetInt(keyForCurrency, m_currentAmount);
        }
        
        private void SetFirstGameAmount()
        {
            var gameManager = DI.Resolve<GameManager>();
            var firstGameAmount = gameManager.GameParameters.InitialCurrency;
            
            PlayerPrefs.SetInt(keyForCurrency, firstGameAmount);
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<CurrencyEarnedEvent>(OnCurrencyEarned);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelSuccess);
        }
    }
}